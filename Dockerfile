# Etapa 1: Construcción de la aplicación
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /home/build

# Copia los archivos de proyecto y el de solución
# Esto permite cachear la capa de restauración de paquetes de manera más efectiva
COPY EduZasAPI.sln .
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/InterfaceAdapters/InterfaceAdapters.csproj src/InterfaceAdapters/
COPY src/Infraestructure/BCrypt/BCrypt.csproj src/Infraestructure/BCrypt/
COPY src/Infraestructure/EntityFramework/EntityFramework.csproj src/Infraestructure/EntityFramework/
COPY src/Infraestructure/FluentValidation/FluentValidationProj.csproj src/Infraestructure/FluentValidation/
COPY src/Infraestructure/Mariadb/Mariadb.csproj src/Infraestructure/Mariadb/
COPY src/Infraestructure/MinimalAPI/MinimalAPI.csproj src/Infraestructure/MinimalAPI/
COPY src/Infraestructure/MailKit/MailKitProj.csproj src/Infraestructure/MailKit/
COPY src/Infraestructure/Composition/Composition.csproj src/Infraestructure/Composition/
COPY src/Infraestructure/Cli/Cli.csproj src/Infraestructure/Cli/

# Eliminar los test de la solucion
RUN dotnet sln remove Tests/

# Restaura las dependencias de la solución
RUN dotnet restore EduZasAPI.sln

# Copia el resto del código fuente
COPY src/ src/

# Publica únicamente el proyecto de entrada (API)
RUN dotnet publish "src/Infraestructure/MinimalAPI/MinimalAPI.csproj" -c ${BUILD_CONFIGURATION} -o /home/build/dist/api
RUN dotnet publish "src/Infraestructure/Cli/Cli.csproj" -c ${BUILD_CONFIGURATION} -o /home/build/dist/cli

# Etapa 2: Imagen final para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /home/app

# Instala el cliente de MariaDB para backups y comandos
RUN apk add --no-cache mariadb-client

# Establece las variables de entorno para las rutas de los binarios de MariaDB
ENV DatabaseBinaries__DumpPath="/usr/bin/mariadb-dump"
ENV DatabaseBinaries__MariadbPath="/usr/bin/mariadb"

# Crea un usuario no-root para mayor seguridad
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser

# Expone el puerto 8080, el estándar para contenedores ASP.NET Core
EXPOSE 8080

# Copia la aplicación publicada desde la etapa de construcción
COPY --from=build /home/build/dist/api/ ./api/
COPY --from=build /home/build/dist/cli/ ./cli/

# Define el punto de entrada
# No se define un ENTRYPOINT para poder ejecutar tanto la API como el CLI
# Para ejecutar la API: dotnet api/MinimalAPI.dll
# Para ejecutar el CLI: dotnet cli/Cli.dll user:reset-password --email <email>
