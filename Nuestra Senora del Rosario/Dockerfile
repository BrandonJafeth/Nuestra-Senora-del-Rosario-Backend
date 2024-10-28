# Imagen base para ASP.NET en Linux
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Imagen SDK para construir el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar todos los archivos de proyecto
COPY ["Nuestra Senora del Rosario.csproj", "."]

# Restaurar dependencias para todos los proyectos
RUN dotnet restore "./Nuestra Senora del Rosario.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar la aplicación
RUN dotnet build "./Nuestra Senora del Rosario.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "./Nuestra Senora del Rosario.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nuestra Senora del Rosario.dll"]
