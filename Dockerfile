# Etapa 1: Construcción de la aplicación
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /home/build

# Copia el archivo de solución y restaura las dependencias de todos los proyectos
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

## Eliminar los tests de la solucion
RUN dotnet sln EduZasAPI.sln remove tests/ApplicationTest/ApplicationTest.csproj
RUN dotnet sln EduZasAPI.sln remove tests/Infraestructure/EntityFrameworkTest/EntityFrameworkTest.csproj
RUN dotnet sln EduZasAPI.sln remove tests/Infraestructure/FluentValidationTest/FluentValidationTest.csproj

# Restaura las dependencias de la solución
RUN dotnet restore EduZasAPI.sln

# Copia el resto del código y publica la aplicación
COPY . .


RUN dotnet publish EduZasAPI.sln -c Release -o ./dist

# Etapa 2: Imagen final para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /home/app

# Instala el cliente de MariaDB para backups y comandos
RUN apk add --no-cache mariadb-client

# Crea un usuario no-root para mayor seguridad
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser

# Expone el puerto que usa la aplicación
EXPOSE 5018

# Copia la aplicación publicada desde la etapa de construcción
COPY --from=build /home/build/dist/ .

# Define el punto de entrada
ENTRYPOINT ["dotnet", "EduZasAPI.Infraestructure.MinimalAPI.dll"]
