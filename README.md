# FinanKey 💰

Aplicación móvil multiplataforma para la gestión de finanzas personales. Permite registrar tarjetas de crédito/débito, transacciones con categorización automática mediante IA, autenticación biométrica y visualización de resúmenes financieros.

> Construida con **.NET 10 MAUI**, siguiendo principios de **Clean Architecture**, **CQRS** y **MVVM**.

---

## 🏗️ Arquitectura del Proyecto

El proyecto sigue una arquitectura limpia (Clean Architecture) con separación clara de responsabilidades:

```
┌─────────────────────────────────────────────────────────┐
│              Presentación (MAUI UI)                      │
│  ViewModels, Pages, Convertidores, Estilos               │
│  ─────────────────────────────────────────────────────  │
│  • MVVM con CommunityToolkit.Mvvm                        │
│  • Syncfusion UI Components                              │
│  • Navegación con AppShell (4 tabs)                      │
└──────────────────────┬──────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────┐
│              Aplicación (Casos de Uso)                   │
│  Comandos, Consultas, DTOs, Servicios                    │
│  ─────────────────────────────────────────────────────  │
│  • Patrón CQRS con MediatR                               │
│  • Handlers independientes por operación                 │
│  • DTOs para transferencia de datos                      │
└──────────────────────┬──────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────┐
│                   Dominio (Core)                         │
│  Entidades, Enumeraciones, Interfaces                    │
│  ─────────────────────────────────────────────────────  │
│  • Entidades: Tarjeta, Transaccion                       │
│  • Interfaces de repositorios y servicios                │
│  • Sin dependencias externas                             │
└──────────────────────┬──────────────────────────────────┘
                       ▲
                       │
┌──────────────────────┴──────────────────────────────────┐
│              Infraestructura (Servicios)                 │
│  Repositorios, Biometría, Predicción IA                  │
│  ─────────────────────────────────────────────────────  │
│  • SQLite (sqlite-net-pcl)                               │
│  • ONNX Runtime (predicción ML local)                    │
│  • Biometría nativa (Face ID / Huella)                   │
└─────────────────────────────────────────────────────────┘
```

### Capas del Proyecto

| Proyecto | Descripción |
|---|---|
| `FinanzasApp.Domain` | Entidades de dominio, enumeraciones y contratos de interfaz. Sin dependencias externas. |
| `FinanzasApp.Aplicacion` | Casos de uso implementados con CQRS (comandos y consultas), DTOs y servicios de aplicación. |
| `FinanzasApp.Infrastructure` | Implementaciones concretas: repositorios SQLite, servicio biométrico, motor de predicción ONNX. |
| `FinanzasApp` (Presentación) | Aplicación MAUI: ViewModels, páginas XAML, convertidores, navegación y configuración DI. |

---

## 📱 Funcionalidades

- **Gestión de tarjetas**: Registro de tarjetas de crédito y débito con límite, saldo, fecha de corte y vencimiento.
- **Registro de transacciones**: Gastos e ingresos con categoría, descripción y notas.
- **Categorización automática con IA**: Predicción de categoría basada en la descripción usando modelo ONNX local.
- **Resumen financiero**: Dashboard con balance total, gastos/ingresos del mes, gráficas por categoría.
- **Resumen de corte**: Cálculo de período de corte para tarjetas de crédito (día de corte, día de pago, próximo vencimiento).
- **Autenticación biométrica**: Acceso seguro con Face ID (iOS), huella digital (Android) o Windows Hello.
- **Historial completo**: Lista de transacciones agrupadas por fecha con búsqueda.
- **Persistencia local**: Todos los datos se almacenan en SQLite en el dispositivo (sin servidor externo).

---

## 🛠️ Tecnologías y Dependencias

### Requisitos Previos

| Herramienta | Versión Mínima |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | **10.0** |
| [Visual Studio 2022](https://visualstudio.microsoft.com/) | 17.12+ con carga de trabajo **.NET MAUI** |
| [VS Code](https://code.visualstudio.com/) (opcional) | Con extensión C# Dev Kit |
| Android SDK | API 35+ (para emulador/dispositivo Android) |
| Xcode | 16+ (para builds en iOS/macOS, solo en macOS) |

### Dependencias NuGet por Proyecto

#### Domain (`FinanzasApp.Domain`)
> Sin dependencias externas — capa pura de dominio.

#### Application (`FinanzasApp.Aplicacion`)
| Paquete | Versión | Uso |
|---|---|---|
| `CommunityToolkit.Mvvm` | 8.4.1 | MVVM helpers, ObservableProperty, RelayCommand |
| `MediatR` | 14.1.0 | Patrón CQRS — despacho de comandos y consultas |
| `Microsoft.Maui.Controls` | $(MauiVersion) | Componentes base de MAUI |

#### Infrastructure (`FinanzasApp.Infrastructure`)
| Paquete | Versión | Uso |
|---|---|---|
| `sqlite-net-pcl` | 1.10.196-beta | ORM asíncrono para SQLite |
| `Microsoft.ML.OnnxRuntime` | 1.24.3 | Inferencia ML local para predicción de categorías |
| `Oscore.Maui.Biometric` | 2.4.1 | Autenticación biométrica multiplataforma |
| `Microsoft.Maui.Controls` | $(MauiVersion) | Componentes base de MAUI |

#### Presentation (`FinanzasApp`)
| Paquete | Versión | Uso |
|---|---|---|
| `CommunityToolkit.Maui` | 14.0.1 | Behaviors, converters y extensions para MAUI |
| `MediatR` | 14.1.0 | CQRS — resolución de handlers |
| `Microsoft.Maui.Controls` | 10.0.50 | Framework MAUI |
| `Microsoft.Extensions.Logging.Debug` | 10.0.0 | Logging para debugging |
| `Oscore.Maui.Biometric` | 2.4.1 | Autenticación biométrica |
| `Plugin.Maui.Biometric` | 0.1.0 | Plugin adicional de biometría |
| `Syncfusion.Maui.Inputs` | 33.1.44 | Componentes de entrada (autocompletado, etc.) |
| `Syncfusion.Maui.Toolkit` | 1.0.9 | Toolkit de componentes Syncfusion |

> **Nota**: Syncfusion requiere registro de licencia. La clave se configura en `App.xaml.cs`.

### Plataformas Target

- **Android**: `net10.0-android`
- **iOS**: `net10.0-ios`
- **macOS (Mac Catalyst)**: `net10.0-maccatalyst`
- **Windows**: `net10.0-windows10.0.19041.0`

---

## 🗄️ Base de Datos

### Motor: **SQLite** (local, embebida)

- **Librería**: `sqlite-net-pcl` (API asíncrona)
- **Archivo**: `finanzas.db` almacenado en `FileSystem.AppDataDirectory`
- **Modo**: WAL (Write-Ahead Logging) para mejor rendimiento
- **Sin migraciones formales**: Se usa `CreateTableAsync<T>()` que es idempotente — agrega columnas nuevas sin afectar existentes.

### Esquema

#### Tabla: `Tarjeta`
| Columna | Tipo | Descripción |
|---|---|---|
| `Id` | int (PK) | Identificador único |
| `Nombre` | string | Nombre de la tarjeta |
| `UltimosDigitos` | string | Últimos 4 dígitos |
| `Tipo` | int (enum) | Credito=0, Debito=1 |
| `ColorHex` | string | Color en formato HEX |
| `Banco` | string | Entidad emisora |
| `LimiteCredito` | decimal? | Límite (solo crédito) |
| `SaldoActual` | decimal | Saldo actual |
| `FechaRegistro` | DateTime | Fecha de creación |
| `EstaActiva` | bool | Estado (soft delete) |
| `RedTarjeta` | string | Visa, Mastercard, etc. |
| `MesVencimiento` | int | Mes de vencimiento |
| `AnioVencimiento` | int | Año de vencimiento |
| `DiaCorte` | int? | Día de corte (crédito) |
| `DiaPago` | int? | Día de pago (crédito) |

#### Tabla: `Transaccion`
| Columna | Tipo | Descripción |
|---|---|---|
| `Id` | int (PK) | Identificador único |
| `TarjetaId` | int (FK) | Referencia a Tarjeta |
| `Descripcion` | string | Descripción del movimiento |
| `Monto` | decimal | Monto (siempre positivo) |
| `Tipo` | int (enum) | Gasto=0, Ingreso=1 |
| `Categoria` | int (enum) | 18 categorías predefinidas |
| `CategoriaPredicha` | int? | Categoría predicha por IA |
| `ConfianzaPrediccion` | float? | Confianza de la predicción (0-1) |
| `Fecha` | DateTime | Fecha de la transacción |
| `Notas` | string? | Notas adicionales |

### Categorías de Transacción

`Alimentacion`, `Transporte`, `Entretenimiento`, `Salud`, `Educacion`, `Hogar`, `Ropa`, `Tecnologia`, `Viajes`, `Servicios`, `Restaurantes`, `Deportes`, `Suscripciones`, `SalarioSueldo`, `Freelance`, `Inversiones`, `Reembolsos`, `Otros`

---

## 🤖 Inteligencia Artificial (Predicción de Categorías)

La aplicación incluye un modelo de **Machine Learning embebido** para predecir la categoría de una transacción basándose en su descripción.

- **Motor**: `Microsoft.ML.OnnxRuntime` — inferencia local, sin conexión a internet.
- **Modelos**: `gastos_model.onnx` (13 categorías de gasto) e `ingresos_model.onnx` (4 categorías de ingreso).
- **Preprocesamiento**: Bag of Words con vocabulario hash-based (500 features) + normalización L2.
- **Post-procesamiento**: Softmax sobre los logits con umbral de confianza de **0.60**.
- **Primera ejecución**: Los archivos `.onnx` se copian del bundle al `AppDataDirectory` automáticamente.
- **Debounce**: 600ms entre predicciones mientras el usuario escribe.

---

## 🔐 Autenticación Biométrica

- **Android**: Huella digital (Fingerprint).
- **iOS**: Face ID / Touch ID (detectado vía `LocalAuthentication`).
- **Persistencia**: El estado (habilitado/deshabilitado) se guarda en `Preferences.Default`.
- **Flujo**: Al iniciar la app, si la biometría está habilitada, se muestra la pantalla `LoginBiometricoPage` como puerta de acceso.

---

## 🚀 Cómo Ejecutar el Proyecto

### 1. Clonar el Repositorio

```bash
git clone <url-del-repo>.git
cd FinanzasApp
```

### 2. Instalar el SDK

Descarga e instala **.NET 10 SDK** desde [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).

```bash
dotnet --version  # Debe mostrar 10.x.x
```

### 3. Restaurar Dependencias

```bash
dotnet restore
```

Esto descargará automáticamente todos los paquetes NuGet listados en los archivos `.csproj`.

### 4. Ejecutar la Aplicación

#### Con Visual Studio 2022 (Recomendado)
1. Abre `FinanzasApp.slnx` en Visual Studio 2022.
2. Selecciona el proyecto de inicio (framework MAUI).
3. Elige el dispositivo/emulador en la barra superior.
4. Presiona **F5** para depurar o **Ctrl+F5** para ejecutar sin depurar.

#### Con CLI (.NET MAUI)

**Android (emulador o dispositivo conectado):**
```bash
dotnet build -t:Run -f net10.0-android
```

**Windows:**
```bash
dotnet build -t:Run -f net10.0-windows10.0.19041.0
```

**iOS/macOS** (solo en macOS con Xcode instalado):
```bash
dotnet build -t:Run -f net10.0-ios
dotnet build -t:Run -f net10.0-maccatalyst
```

#### Con Visual Studio Code
1. Abre la carpeta del proyecto en VS Code.
2. Instala la extensión **C# Dev Kit**.
3. Usa `Ctrl+Shift+P` → `.NET: Run` y selecciona la plataforma.

### 5. Configuración Adicional

- **Licencia Syncfusion**: El archivo `App.xaml.cs` contiene una clave de licencia. Si usas una versión diferente de Syncfusion, actualiza la clave o regístrate en [syncfusion.com](https://www.syncfusion.com/).
- **Modelos ONNX**: Los archivos `gastos_model.onnx` e `ingresos_model.onnx` deben estar en el proyecto como `MauiAsset` con `CopyToOutput=Always`.
- **Biometría en emulador Android**: Configura un sensor biométrico en las opciones avanzadas del emulador (Extended Controls > Security > Fingerprint).

---

## 📁 Estructura de Carpetas

```
FinanzasApp/
├── FinanzasApp.Domain/                # Capa de Dominio
│   ├── Entidades/
│   │   ├── Tarjeta.cs
│   │   └── Transaccion.cs
│   └── Enumeraciones/
│       └── Enumeraciones.cs
│   └── Interfaces/
│       ├── IRepositorios.cs
│       ├── IServicioBiometrico.cs
│       └── IServicioPrediccion.cs
│
├── FinanzasApp.Aplicacion/            # Capa de Aplicación
│   ├── DTOs/
│   ├── Interfaces/
│   │   └── IMediator.cs
│   ├── Tarjetas/
│   │   ├── Comandos/
│   │   ├── Consultas/
│   │   └── Servicios/
│   └── Transacciones/
│       ├── TransaccionComandos.cs
│       └── TransaccionConsultas.cs
│
├── FinanzasApp.Infrastructure/        # Capa de Infraestructura
│   ├── Biometria/
│   │   └── ServicioBiometricoMaui.cs
│   ├── Persistencia/
│   │   ├── ContextoBD.cs
│   │   └── Repositorios/
│   │       └── Repositorios.cs
│   └── Prediccion/
│       └── ServicioPrediccionOnnx.cs
│
├── FinanzasApp/                       # Capa de Presentación (MAUI)
│   ├── Platforms/                     # Código específico por plataforma
│   ├── Resources/                     # Imágenes, fuentes, estilos
│   ├── Views/
│   │   ├── Pages/
│   │   └── Convertidores/
│   ├── ViewModels/
│   ├── App.xaml.cs
│   ├── AppShell.xaml.cs
│   └── MauiProgram.cs
│
├── FinanzasApp.slnx                   # Solución
├── .gitignore
└── README.md
```

---

## 🎨 UI/UX

### Navegación (AppShell — 4 Tabs)

| Tab | Página | ViewModel | Descripción |
|---|---|---|---|
| 🏠 **Inicio** | `DashboardPage` | `DashboardViewModel` | Resumen financiero, carrusel de tarjetas, transacciones recientes |
| 💳 **Tarjetas** | `TarjetasPage` | `TarjetasViewModel` | Lista de tarjetas agrupadas por tipo, swipe-to-delete |
| 📋 **Movimientos** | `TransaccionesGlobalPage` | `MovimientosViewModel` | Historial completo con búsqueda y agrupación por fecha |
| ⚙️ **Ajustes** | `ConfiguracionPage` | `ConfiguracionViewModel` | Biometría, versión de app, limpiar datos |

### Páginas de Formulario

- `TarjetaFormPage` — Crear/editar tarjeta con validación inline, selector de color, campos para banco, dígitos, fechas y saldo.
- `TransaccionFormPage` — Crear/editar transacción con autocompletado de categoría vía IA, chips de categoría, debounce de 600ms.
- `TarjetaDetallePage` — Detalle de tarjeta con sus transacciones filtradas y resumen de período de corte.

### Tipografía

- **Inter** — Texto principal
- **Poppins** — Títulos y encabezados
- **OpenSans** — Fallback/estándar

---

## 🧩 Conceptos Clave

### CQRS (Command Query Responsibility Segregation)
Patrón que separa las operaciones de **escritura** (Comandos) de las de **lectura** (Consultas). Cada comando/consulta tiene su handler independiente, lo que facilita testing, caching y optimización.

### MediatR
Librería que implementa el patrón **Mediator**. Los ViewModels envían comandos/consultas sin conocer al receptor directo — MediatR resuelve el handler correspondiente vía Dependency Injection.

### Período de Corte
En tarjetas de crédito, el **día de corte** marca el fin de un ciclo de facturación. El **día de pago** es la fecha límite para pagar. `ServicioPeriodoCorte` calcula automáticamente estos períodos.

### Predicción Optimista
Cuando el usuario escribe una descripción, la IA predice la categoría con un debounce de 600ms. Si la confianza supera 0.60, la categoría se pre-selecciona (estado "optimista") pero el usuario puede cambiarla.

### Soft Delete
Las tarjetas no se eliminan físicamente — se marca `EstaActiva = false` para preservar el historial de transacciones asociadas.

---

## 🐛 Troubleshooting

| Problema | Solución |
|---|---|
| `error NETSDK1139: The SDK 'Microsoft.NET.Sdk.Maui' could not be found` | Instala la carga de trabajo **.NET MAUI** en Visual Studio Installer o ejecuta `dotnet workload install maui` |
| `A valid Android package was not found` | Configura un emulador Android en Android Studio o conecta un dispositivo físico con depuración USB habilitada |
| `Biometric authentication not available` | En emulador Android: Extended Controls > Security > configura fingerprint. En iOS: verifica permisos en Info.plist |
| `Syncfusion license error` | Regístrate en syncfusion.com para una clave gratuita de comunidad o elimina la llamada `Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense()` |
| `ONNX model not found` | Verifica que `gastos_model.onnx` e `ingresos_model.onnx` estén en `Resources/Raw/` con `Build Action = MauiAsset` |
| `The type or namespace 'MediatR' could not be found` | Ejecuta `dotnet restore` para descargar los paquetes NuGet |

---

## 📄 Licencia

Este proyecto es para uso personal y educativo. Las dependencias utilizadas tienen sus propias licencias:

- **Syncfusion**: Licencia de comunidad disponible en [syncfusion.com](https://www.syncfusion.com/products/communitylicense)
- **ONNX Runtime**: Licencia MIT
- **CommunityToolkit**: Licencia MIT
- **sqlite-net-pcl**: Licencia BSD

---

## 🤝 Contribuir

1. Haz fork del repositorio
2. Crea una rama (`git checkout -b feature/nueva-funcionalidad`)
3. Realiza tus cambios y haz commit (`git commit -m 'feat: agrega nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Abre un Pull Request

---

<div align="center">

**Hecho con ❤️ usando .NET MAUI**

[.NET](https://dotnet.microsoft.com/) · [MAUI](https://dotnet.microsoft.com/apps/maui) · [Syncfusion](https://www.syncfusion.com/)

</div>
