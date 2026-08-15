# Sistema de Administración de Clientes

Aplicación web de extremo a extremo para registrar, consultar, modificar y dar de baja clientes.

**Stack:** SQL Server · .NET 8 · C# · Entity Framework Core · Angular 18

## Arquitectura

```
Clientes.sln
├── Clientes.DataAccess        Persistencia (EF Core, repositorios)
├── Clientes.BusinessLogic     Reglas de negocio, DTOs y validaciones
├── Clientes.Service           API REST (controladores, CORS, Swagger)
└── Clientes.BusinessLogic.Tests
frontend/                      Angular 18
database/                      Script SQL y datos de ejemplo
postman/                       Colección de pruebas HTTP
docs/                          Documento técnico
```

Dependencias:

- `Clientes.Service` → `Clientes.BusinessLogic` y `Clientes.DataAccess`
- `Clientes.BusinessLogic` → `Clientes.DataAccess`

## Requisitos

- SQL Server (local o Express) con autenticación de Windows, o una cadena de conexión equivalente
- SDK de .NET 8 o posterior (el proyecto usa `net8.0`)
- Node.js 18 o posterior
- Angular CLI no es obligatorio: se usa `npx`/`npm`

## 1. Base de datos

Use **una sola** de las dos opciones siguientes (no combine el script SQL con `database update`).

Opción A — script SQL:

```powershell
sqlcmd -S localhost -E -i database\ClientesDb.sql
sqlcmd -S localhost -E -d ClientesDb -i database\Seed.sql
```

Opción B — migraciones de Entity Framework Core:

```powershell
dotnet tool restore
dotnet ef database update --project Clientes.DataAccess --startup-project Clientes.Service
```

Si SQL Server no está en `localhost`, ajuste `Clientes.Service/appsettings.Development.json` o la variable de entorno `ConnectionStrings__ClientesDb`.

## 2. Cadena de conexión

En desarrollo se usa autenticación integrada (sin contraseña):

```
Server=localhost;Database=ClientesDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True
```

Para otro entorno, no incluya secretos en el repositorio. Ejemplos:

```powershell
$env:ConnectionStrings__ClientesDb = "Server=...;Database=ClientesDb;User Id=...;Password=...;TrustServerCertificate=True"
dotnet user-secrets init --project Clientes.Service
dotnet user-secrets set "ConnectionStrings:ClientesDb" "Server=...;Database=ClientesDb;..." --project Clientes.Service
```

## 3. Backend

```powershell
dotnet restore Clientes.sln
dotnet test Clientes.sln
dotnet run --project Clientes.Service --launch-profile http
```

- API: http://localhost:5216
- Swagger: http://localhost:5216/swagger

### Endpoints

| Método | Ruta | Resultado esperado |
| --- | --- | --- |
| GET | `/api/clientes` | 200 listado paginado |
| GET | `/api/clientes/{id}` | 200 o 404 |
| POST | `/api/clientes` | 201, 400 o 409 |
| PUT | `/api/clientes/{id}` | 200, 400, 404 o 409 |
| DELETE | `/api/clientes/{id}` | 204, 404 o 409 (baja lógica) |

Consulta con filtros:

```
GET /api/clientes?pagina=1&tamanioPagina=10&buscar=David&activo=true
```

## 4. Frontend

```powershell
cd frontend
npm install
npm start
```

Abra http://localhost:4200

La URL de la API se configura en:

- `frontend/src/environments/environment.development.ts` (ng serve)
- `frontend/src/environments/environment.ts` (producción)

## 5. Pruebas

```powershell
dotnet test Clientes.sln
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless
```

En Postman importe `postman/Clientes.postman_collection.json`.

## Librerías externas

Backend: Entity Framework Core 8, Swashbuckle, Moq, xUnit.

Frontend: Angular 18, Angular Material 18, Angular CDK, RxJS.

## Decisiones relevantes

- La baja es lógica: `Activo = 0` y se actualiza `FechaModificacion`. No se borra el registro.
- Las fechas de auditoría las asigna el backend en UTC.
- Las entidades de EF Core no se exponen en la API; se usan DTOs.
- Las lecturas usan `AsNoTracking`.
- La paginación, búsqueda, filtro de estado y ordenamiento ocurren en el servidor.
- Los errores se responden con `ProblemDetails` y no exponen stack traces ni SQL.

El detalle técnico está en [docs/DOCUMENTO_TECNICO.md](docs/DOCUMENTO_TECNICO.md).
