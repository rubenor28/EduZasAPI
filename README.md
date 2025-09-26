# EduZasAPI

API para gestión del sistema educativo EduZas

## Prerequisitos

### Opción 1: Dev Container (Recomendado para desarrollo)

- **Visual Studio Code** con la extensión **Dev Containers**
- **Docker** instalado en tu sistema

### Opción 2: Manual

- **MySQL** ≥ 5.7.9 GA o **MariaDB** ≥ 10.4.3
- **.NET 9.0 SDK**
- **ASP.NET Core 9.0**

## Estructura del proyecto (Clean Architecture)

El proyecto sigue los principios de Clean Architecture organizándose en las siguientes capas:

```
.
├── src/
│   ├── EduZasAPI.Domain/                               # Capa de dominio (entidades, reglas de negocio, value objects)
│   ├── EduZasAPI.Application/                          # Casos de uso, DTOs y puertos de aplicación
│   └── EduZasAPI.Infraestructure/                      # Implementaciones de infraestructura
│       ├── EduZasAPI.Infraestructure.BCrypt/           # Implementación de hashing
│       ├── EduZasAPI.Infraestructure.EntityFramework/  # Implementación de Entity Framework
│       ├── EduZasAPI.Infraestructure.FluentValidation/ # Implementación de validaciones
│       └── EduZasAPI.Infraestructure.MinimalAPI/       # Entry point de la aplicación
│           ├── Extensions/                             # Configuración de inyección de dependencias
│           ├── Presentation/                           # Rutas y filtros de la API
│           └── Program.cs                              # Punto de inicio
├── tests/                                              # Tests de infraestructura
└── run.sh                                              # Script de ejecución
```

## Desarrollo

### Opción 1: Dev Container (Recomendado)

1. **Abrir con Dev Container:**
   - Asegúrate de tener Docker instalado y ejecutándose
   - Instala la extensión "Dev Containers" en VS Code
   - Abre el proyecto en VS Code
   - Presiona `Ctrl+Shift+P` (o `Cmd+Shift+P` en Mac) y selecciona **"Dev Containers: Reopen in Container"**
   - O haz clic en el botón "Reopen in Container" cuando aparezca en la esquina inferior derecha

2. **El contenedor de desarrollo incluye:**
   - .NET 9.0 SDK preinstalado
   - Todas las dependencias del proyecto configuradas
   - Extensiones útiles de VS Code para desarrollo .NET
   - Entorno de desarrollo consistente para todo el equipo

### Opción 2: Configuración manual

1. Clonar el repositorio:

```bash
git clone https://github.com/tu-usuario/EduZasAPI.git
```

2. Instalar .NET 9.0 SDK desde [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/9.0)

3. Configurar la base de datos. Dentro de `EduZasAPI.Infraestructure.MinimalAPI` existen ejemplos tanto
   para producción como desarrollo, y algunos test también requiren un `appsettings.json`

## Despliegue

### Ejecución local

```bash
# Usar el script proporcionado
./run.sh

# O ejecutar manualmente
dotnet run --project src/EduZasAPI.Infraestructure/EduZasAPI.Infraestructure.MinimalAPI/EduZasAPI.Infraestructure.MinimalAPI.csproj
```

### Comandos de desarrollo

```bash
# Ejecutar tests
dotnet test

# Restaurar dependencias
dotnet restore

# Compilar proyecto
dotnet build

# Ejecutar con hot reload (desarrollo)
dotnet watch run --project src/EduZasAPI.Infraestructure/EduZasAPI.Infraestructure.MinimalAPI/EduZasAPI.Infraestructure.MinimalAPI.csproj
```

### Documentación de la API

Una vez ejecutada la aplicación, la documentación Swagger estará disponible en:

- [http://localhost:5018/swagger/](http://localhost:5018/swagger/)

Accesible desde cualquier navegador web.

### Configuración de dependencias

La inyección de dependencias está configurada en:
`src/EduZasAPI.Infraestructure/EduZasAPI.Infraestructure.MinimalAPI/Extensions/`
