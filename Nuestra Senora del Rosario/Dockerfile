# Imagen base para ASP.NET en Linux
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Instalar dependencias necesarias para Puppeteer en Linux
RUN apt-get update && apt-get install -y \
    libnss3 \
    libatk-bridge2.0-0 \
    libx11-xcb1 \
    libxcomposite1 \
    libxdamage1 \
    libxrandr2 \
    libgbm1 \
    libpango-1.0-0 \
    libcups2 \
    libxss1 \
    libgconf-2-4 \
    libasound2 \
    libatk1.0-0 \
    libgtk-3-0 \
    libxshmfence1 \
    wget \
    && rm -rf /var/lib/apt/lists/*

# Imagen SDK para construir el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar el archivo .csproj desde su ubicación
COPY ["Nuestra Senora del Rosario/Nuestra Senora del Rosario.csproj", "Nuestra Senora del Rosario/"]

# Restaurar dependencias para todos los proyectos
RUN dotnet restore "Nuestra Senora del Rosario/Nuestra Senora del Rosario.csproj"

# Copiar todo el código fuente
COPY . .

# Establecer directorio de trabajo y compilar la aplicación
WORKDIR "/src/Nuestra Senora del Rosario"
RUN dotnet build "Nuestra Senora del Rosario.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "Nuestra Senora del Rosario.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final para ejecutar la aplicación
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configuración para Puppeteer
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-stable"

# Copiar la plantilla HTML a una carpeta conocida dentro del contenedor
COPY ["Nuestra Senora del Rosario/Templates/", "/app/Templates"]

# Entrypoint
ENTRYPOINT ["dotnet", "Nuestra Senora del Rosario.dll"]
