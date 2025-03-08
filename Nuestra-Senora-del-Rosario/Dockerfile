# Imagen base para ASP.NET en Linux
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Puerto por defecto
EXPOSE 10000

# ------------------------------------------------
# Instalar dependencias necesarias para Puppeteer
# y Google Chrome (fuentes, librerías, etc.).
# ------------------------------------------------
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

# Agregar repositorio de Google Chrome y su clave
RUN wget -qO- https://dl.google.com/linux/linux_signing_key.pub | \
    gpg --dearmor > /usr/share/keyrings/google-chrome.gpg && \
    echo "deb [signed-by=/usr/share/keyrings/google-chrome.gpg] \
    http://dl.google.com/linux/chrome/deb/ stable main" \
    > /etc/apt/sources.list.d/google-chrome.list && \
    apt-get update && apt-get install -y google-chrome-stable && \
    rm -rf /var/lib/apt/lists/*

# Variable de entorno para Puppeteer
ENV PUPPETEER_EXECUTABLE_PATH="/usr/bin/google-chrome-stable"

# ------------------------------------------------
# Imagen SDK para compilar (dotnet sdk:8.0)
# ------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1) Copiar el .csproj
# Asegúrate de que en tu repo la carpeta se llame
# "Nuestra-Senora-del-Rosario" y el proyecto se llame
# "Nuestra-Senora-del-Rosario.csproj"
COPY ["Nuestra-Senora-del-Rosario/Nuestra-Senora-del-Rosario.csproj", "Nuestra-Senora-del-Rosario/"]

# 2) Restaurar dependencias
RUN dotnet restore "Nuestra-Senora-del-Rosario/Nuestra-Senora-del-Rosario.csproj"

# 3) Copiar todo el código fuente
COPY . .

# 4) Compilar la aplicación
WORKDIR "/src/Nuestra-Senora-del-Rosario"
RUN dotnet build "Nuestra-Senora-del-Rosario.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# ------------------------------------------------
# Publicar la aplicación
# ------------------------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Nuestra-Senora-del-Rosario.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ------------------------------------------------
# Imagen final para ejecutar la aplicación
# ------------------------------------------------
FROM base AS final
WORKDIR /app

# 1) Copiar la carpeta publish
COPY --from=publish /app/publish .

# 2) Copiar la carpeta Templates (si existe)
# Si tu repo tiene "Templates" dentro de "Nuestra-Senora-del-Rosario",
# esta línea lo copia a /app/Templates en el contenedor.
COPY ["Nuestra-Senora-del-Rosario/Templates/", "/app/Templates"]

# 3) Iniciar la aplicación
ENTRYPOINT ["dotnet", "Nuestra-Senora-del-Rosario.dll"]
