# Documento técnico

## Sistema de Administración de Clientes

**Candidato / solución:** Examen técnico Desarrollador Senior .NET + Angular  
**Stack:** SQL Server, .NET 8, C#, EF Core, ASP.NET Core Web API, Angular 18

## 1. Arquitectura y responsabilidad de cada proyecto

La solución usa tres proyectos de backend reales (no carpetas dentro de un solo proyecto), más el frontend y las pruebas:

| Proyecto | Responsabilidad |
| --- | --- |
| `Clientes.DataAccess` | `DbContext`, entidad `Cliente`, configuración Fluent API, repositorio asíncrono. No contiene reglas de negocio. |
| `Clientes.BusinessLogic` | Servicios, interfaces, DTOs, validaciones, control de duplicados, baja lógica y fechas de auditoría. |
| `Clientes.Service` | API REST, inyección de dependencias, Swagger, CORS, manejador global de excepciones. |
| `Clientes.BusinessLogic.Tests` | Pruebas unitarias de las reglas críticas. |
| `frontend` | Angular 18: listado, formulario reactivo y consumo exclusivo de la API. |

Flujo de una petición:

`Angular 18 → API REST → ClienteService (negocio) → IClienteRepository → EF Core → SQL Server`

`Clientes.Service` referencia a negocio y a datos porque ahí se registran `DbContext` y el repositorio. El controlador no usa EF Core ni entidades de persistencia.

## 2. Modelo de datos

Tabla `dbo.Clientes`:

- `ClienteId` identidad, clave primaria
- `Nombre` y `ApellidoPaterno` obligatorios (100)
- `ApellidoMaterno` opcional (100)
- `CorreoElectronico` obligatorio, único (200)
- `Telefono` (20), `FechaNacimiento` (`date`), `Direccion` (250), `Ciudad` (100), `CodigoPostal` (10)
- `Activo` bit, default 1, usado para baja lógica
- `FechaRegistro` y `FechaModificacion` (`datetime2`)

Índices: único por correo (`UQ_Clientes_Correo`) e índice de consulta `IX_Clientes_Nombre` sobre apellido paterno y nombre.

## 3. Instalación y configuración

Requisitos: SQL Server, SDK compatible con `net8.0`, Node.js 18+.

1. Restaurar y compilar `Clientes.sln`.
2. Crear la base con `database/ClientesDb.sql` o con `dotnet ef database update`.
3. Confirmar la cadena `ConnectionStrings:ClientesDb` (archivo de desarrollo, variable de entorno o user secrets).
4. Ejecutar la API en http://localhost:5216.
5. En `frontend`, `npm install` y `npm start` (http://localhost:4200).
6. CORS permite el origen del frontend; la URL de la API se cambia por ambiente en `src/environments`.

## 4. Creación o actualización de la base de datos

Hay dos caminos equivalentes:

- Script `database/ClientesDb.sql` (idempotente si la base o la tabla ya existen).
- Migraciones de EF Core en `Clientes.DataAccess/Migrations`.

`database/Seed.sql` inserta dos clientes de ejemplo para la demostración.

## 5. Ejecución del backend y frontend

```powershell
dotnet run --project Clientes.Service --launch-profile http
cd frontend
npm start
```

Swagger queda en `/swagger`. La UI consume únicamente `/api/clientes`.

## 6. Endpoints disponibles

| Método | Endpoint | Descripción | Códigos |
| --- | --- | --- | --- |
| GET | `/api/clientes` | Listado paginado, búsqueda, filtro `activo`, `ordenarPor`, `descendente` | 200, 400 |
| GET | `/api/clientes/{id}` | Consulta individual | 200, 404 |
| POST | `/api/clientes` | Alta | 201, 400, 409 |
| PUT | `/api/clientes/{id}` | Modificación | 200, 400, 404, 409 |
| DELETE | `/api/clientes/{id}` | Baja lógica (`Activo = 0`) | 204, 404, 409 |

Ejemplo de alta:

```json
{
  "nombre": "Laura",
  "apellidoPaterno": "Martínez",
  "apellidoMaterno": "Gómez",
  "correoElectronico": "laura.martinez@correo.com",
  "telefono": "3312345678",
  "fechaNacimiento": "1990-05-18",
  "direccion": "Av. Vallarta 1500",
  "ciudad": "Guadalajara",
  "codigoPostal": "44110"
}
```

## 7. Validaciones y reglas de negocio

- Nombre y apellido paterno obligatorios.
- Correo obligatorio, con formato válido y único (comparación normalizada a minúsculas).
- Longitudes máximas alineadas con SQL Server.
- La fecha de nacimiento no puede ser futura.
- No se modifica ni se da de baja un cliente inexistente (404).
- Si el cliente ya está inactivo, la baja devuelve 409.
- `FechaRegistro` y `FechaModificacion` las asigna el backend en UTC; no viajan en el request de escritura.
- La interfaz pide confirmación antes de la baja.

Las validaciones existen en Angular (formularios reactivos) y se vuelven a aplicar en `ClienteValidador` del backend.

## 8. Manejo de errores

`ManejadorExcepcionesGlobal` (`IExceptionHandler`) convierte excepciones de negocio en `ProblemDetails`:

- 400 `ValidacionNegocioException`, con diccionario `errors`
- 404 `RecursoNoEncontradoException`
- 409 `ConflictoNegocioException` o violación de índice único (SQL 2601/2627)
- 500 genérico, sin stack trace, SQL ni detalles internos

El interceptor HTTP de Angular muestra el `detail` (o el primer error de validación) y cubre el caso de API caída.

## 9. Decisiones técnicas y supuestos

1. **Baja lógica:** se prefirió `Activo = 0` frente a un `DELETE` físico para conservar historial y respetar el modelo pedido. El endpoint sigue siendo DELETE porque es el contrato REST del ejercicio.
2. **Capas:** repositorio en DataAccess, reglas en BusinessLogic, API delgada. Inversión de dependencias con `IClienteRepository` e `IClienteService`.
3. **Sin AutoMapper:** un solo agregado; el mapeo manual es explícito y evita una dependencia extra.
4. **Lecturas con `AsNoTracking`:** las escrituras cargan el agregado con seguimiento.
5. **Paginación en servidor:** evita traer la tabla completa. Tamaño máximo 100.
6. **UTC en auditoría:** independiente de la zona del servidor; el script SQL usa `SYSUTCDATETIME()` para alinearse.
7. **Secretos:** `appsettings.Development.json` usa Windows Authentication. Credenciales reales deben ir a user secrets o variables de entorno.
8. **Supuesto de entorno:** SQL Server accesible en `localhost` y Angular en el puerto 4200.

## 10. Pruebas realizadas

Backend (`dotnet test`):

- Alta válida (cliente activo y fecha de registro asignada)
- Correo duplicado → conflicto
- Nombre vacío y fecha futura → validación
- Modificación de cliente inexistente → 404 de negocio
- Baja lógica marca `Activo = false`
- Baja de inexistente → no encontrado

Frontend (`ClienteService` con `HttpClientTesting`):

- GET con query params de paginación/búsqueda
- POST de alta
- DELETE de baja

Pruebas manuales previstas: Swagger, colección de Postman y recorrido UI (alta, edición, confirmación de baja, búsqueda y paginación).

## 11. Limitaciones y mejoras para producción

- No hay autenticación ni autorización (fuera de alcance).
- No hay auditoría de usuario (quién modificó).
- La unicidad de correo se valida en negocio y en índice; en alta concurrencia el índice es la red de seguridad.
- No hay reactivación de clientes inactivos.
- Faltarían logs estructurados, health checks, observabilidad y un pipeline CI.
- El listado podría ofrecer ordenamiento por columnas en la tabla.
- Para producción conviene HTTPS, rotación de secretos y un origen CORS restringido al dominio real.

## 12. Indicadores aplicados

- `async/await` y `CancellationToken` de punta a punta
- DTOs sin exponer entidades
- `AsNoTracking` en lecturas
- Paginación en servidor
- Manejo global de errores
- Configuración de la cadena de conexión sin secretos reales
- Pruebas unitarias de las reglas pedidas
