
# Imagen base para ASP.NET en Linux
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000  # Cambia esto a 10000 que es el puerto por defecto en Render

# Instalar dependencias necesarias para Puppeteer y Google Chrome
RUN apt-get update && apt-get install -y \
    wget \
    gnupg \
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
    libasound2 \
    libatk1.0-0 \
    libgtk-3-0 \
    libxshmfence1 \
    fonts-liberation \
    fonts-noto \
    fonts-noto-cjk \
    fonts-noto-color-emoji \
    && rm -rf /var/lib/apt/lists/*

# Agregar el repositorio de Google Chrome y su clave
RUN wget -qO- https://dl.google.com/linux/linux_signing_key.pub | gpg --dearmor > /usr/share/keyrings/google-chrome.gpg && \
    echo "deb [signed-by=/usr/share/keyrings/google-chrome.gpg] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list && \
    apt-get update && apt-get install -y google-chrome-stable && \
    rm -rf /var/lib/apt/lists/*

# Configurar la variable de entorno para Puppeteer
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-stable"

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

# Copiar la plantilla HTML a una carpeta conocida dentro del contenedor
COPY ["Nuestra Senora del Rosario/Templates/", "/app/Templates"]

# Entrypoint
ENTRYPOINT ["dotnet", "Nuestra Senora del Rosario.dll"]
