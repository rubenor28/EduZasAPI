# Etapa 1: Construcción de la aplicación
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /home/build

# Copia el archivo de solución y restaura las dependencias de todos los proyectos
COPY EduZasAPI.sln .
COPY src/EduZasAPI.Domain/EduZasAPI.Domain.csproj src/EduZasAPI.Domain/
COPY src/EduZasAPI.Application/EduZasAPI.Application.csproj src/EduZasAPI.Application/
COPY src/EduZasAPI.InterfaceAdapters/EduZasAPI.InterfaceAdapters.csproj src/EduZasAPI.InterfaceAdapters/
COPY src/EduZasAPI.Infraestructure/EduZasAPI.Infraestructure.BCrypt/EduZasAPI.Infraestructure.BCrypt.csproj src/EduZasAPI.Infraestructure/EduZasAPI.Infraestructure.BCrypt/
COPY src/EduZasAPI.Infraestructure/EduZasAPI.Infraestructure.EntityFramework/EduZasAPI.Infraestructure.EntityFramework.csproj src/EduZasAPI.Infraestructure/EduZasAPI.Infraestructure.EntityFramework/
COPY src/EduZasAPI.Infraestructure/EduZasAPI.Infraestructure.FluentValidation/EduZasAPI.Infraestructure.FluentValidation.csproj src/EduZasAPI.Infraestructure/EduZasAPI.Infraestructure.FluentValidation/
COPY src/EduZasAPI.Infraestructure/EduZasAPI.Infraestructure.MinimalAPI/EduZasAPI.Infraestructure.MinimalAPI.csproj src/EduZasAPI.Infraestructure/EduZasAPI.Infraestructure.MinimalAPI/

# Restaura las dependencias de la solución
RUN dotnet restore EduZasAPI.sln

# Copia el resto del código y publica la aplicación
COPY . .
RUN dotnet publish EduZasAPI.sln -c Release -o ./dist

# Etapa 2: Imagen final para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /home/app

# Crea un usuario no-root para mayor seguridad
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser

# Expone el puerto que usa la aplicación
EXPOSE 5018

# Copia la aplicación publicada desde la etapa de construcción
COPY --from=build /home/build/dist/ .

# Define el punto de entrada
ENTRYPOINT ["dotnet", "EduZasAPI.Infraestructure.MinimalAPI.dll"]
