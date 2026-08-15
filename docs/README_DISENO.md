# Identidad visual — Surtidora Departamental

Guía de colorimetría, tipografía y componentes del frontend de Administración de clientes.

Fuente de verdad en código: `frontend/src/styles.scss` y `frontend/src/index.html`.

---

## Colorimetría

La paleta replica la marca de tienda: verde petróleo en superficies de navegación y teal de logo SD en acciones.

| Token | HEX | Uso |
| --- | --- | --- |
| Header / texto principal | `#0c3535` | Barra superior, títulos, color de texto base |
| Teal marca | `#00b4b4` | Badge del logo, botones primarios, conteos, enlaces, chip Activo, foco |
| Teal hover | `#009a9a` | Hover del botón primario y snackbar de éxito |
| Superficie | `#f0f8f8` | Fondo de página (tinte teal muy suave) |
| Blanco | `#ffffff` | Tarjetas, campos y botones secundarios |
| Slate 400 | `#94a3b8` | Textos de apoyo, placeholders, encabezados de tabla |
| Slate 500 | `#64748b` | Etiquetas de formulario, botón secundario |
| Slate 700 | `#334155` | Texto de celdas e inputs |
| Slate 100 | `#f1f5f9` | Bordes de tarjeta, icono de estado vacío |
| Slate 200 | `#e2e8f0` | Borde de campos y botón secundario |
| Slate 50 | `#f8fafc` | Fondo del buscador y encabezado de tabla |
| Error | `#dc2626` | Mensajes de validación en el formulario |

### Cómo se leen

- **`#0c3535`** ancla la marca (oscuro, sobrio).
- **`#00b4b4`** es el único acento; no se mezclan otros primarios (ni azul Material).
- El fondo **`#f0f8f8`** evita el gris frío y alinea la app con la tienda.
- Los slate solo sirven para jerarquía y bordes, no para acciones.

### Variables CSS

```css
:root {
  --app-header: #0c3535;
  --app-teal: #00b4b4;
  --app-teal-dark: #009a9a;
  --app-surface: #f0f8f8;
  --app-muted: #94a3b8;
  --app-text: #0c3535;
  --app-border: #f1f5f9;
}
```

No hardcodear colores en componentes si ya existe token. Excepción: slates de apoyo en filtros y tabla.

---

## Tipografía

| Rol | Familia | Pesos | Dónde |
| --- | --- | --- | --- |
| UI / admin | [Outfit](https://fonts.google.com/specimen/Outfit) | 300, 400, 500, 600 | Body, botones, tabla, formularios, barra |
| Iconos | Material Icons | — | Logo, navegación, filtros, vacío |

En este admin **Outfit cubre todo**, incluidos los títulos (`font-weight: 600`). No se usa serif en pantallas operativas.

Carga:

```
https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600&display=swap
```

### Escala

| Estilo | Tamaño | Peso |
| --- | --- | --- |
| Título de página | 1.5rem (24px) | 600 |
| Marca en header | 0.875rem (14px) | 600 |
| Subtítulo de marca | 10px | 400, 50% blanco |
| Cuerpo | 14px base; 0.875rem en UI | 400 / 500 |
| Navegación header | 0.75rem | 500 |
| Encabezados de tabla | 0.75rem | 600 |
| Etiquetas / meta | 10px – 0.75rem | 500 |
| Botones | 0.875rem | 500 |

`letter-spacing` en títulos: `-0.02em`.

---

## Forma y elevación

| Elemento | Radio | Sombra / borde |
| --- | --- | --- |
| Tarjeta | 16px | `1px #f1f5f9` + sombra muy suave |
| Botón | 12px | Primario: glow teal 18% |
| Campo | 12px | `1px #e2e8f0`; foco: anillo teal 12% |
| Badge logo | 8px | Fondo `#00b4b4` |
| Nav del header | 8px | Activo: blanco 15% |
| Chip de estado | 999px | Fondo `#e6fafa` |
| Icono estado vacío | 16px | Fondo `#f1f5f9` |

Layout: contenido a **80rem** de ancho máximo, padding horizontal **1.5rem**, barra de **56px**.

---

## Componentes

### Header

- Fondo `#0c3535`, sombra `0 8px 24px` al 18% del mismo color.
- Badge 32×32 con icono `groups` en blanco.
- **Listado** / **Nuevo**: texto blanco 60%; activo blanco sobre `white/15`.

### Botones

- **Primario** (`.btn-primario`): `#00b4b4` → hover `#009a9a`. Alta, buscar, guardar.
- **Secundario** (`.btn-secundario`): blanco, borde slate. Limpiar, cancelar.
- **Outline** (`.btn-outline`): borde y texto teal. CTA del estado vacío.

### Campos

- Clase `.campo`. El buscador usa `.campo-buscar` (fondo slate-50 + icono search).
- Foco: borde teal + ring 3px.
- Select de estado: etiqueta flotante de 10px “Estado”.

### Tabla

- Encabezado slate-50 al 70%, texto slate-400.
- Columna acciones: enlaces **Editar** (teal) y **Baja** (slate).
- Chip **Activo** teal; **Inactivo** gris.

### Estado vacío

Icono en recuadro, título semibold slate-700, texto de apoyo slate-400 y botón outline “Crear cliente”.

---

## Clases reutilizables

Definidas en `frontend/src/styles.scss`:

- `.tarjeta`
- `.btn` + `.btn-primario` | `.btn-secundario` | `.btn-outline`
- `.campo` + `.campo-buscar`

Pantallas: `app.component`, `cliente-lista`, `cliente-formulario`.

---

## No hacer

- No introducir un segundo color de acción (azul Material, naranja, etc.).
- No usar otra familia en el admin (Inter, Roboto, serif).
- No aplicar sombras fuertes ni radios de 0; el lenguaje es tarjeta suave y redondeada.
- No poner degradados fuera del header.
