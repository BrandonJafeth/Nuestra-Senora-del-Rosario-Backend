# Base image for ASP.NET on Linux
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

# Install dependencies for Puppeteer and Google Chrome (fonts, libraries, etc.)
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

# Add Google Chrome repo and key
RUN wget -qO- https://dl.google.com/linux/linux_signing_key.pub | \
    gpg --dearmor > /usr/share/keyrings/google-chrome.gpg && \
    echo "deb [signed-by=/usr/share/keyrings/google-chrome.gpg] \
    http://dl.google.com/linux/chrome/deb/ stable main" \
    > /etc/apt/sources.list.d/google-chrome.list && \
    apt-get update && apt-get install -y google-chrome-stable && \
    rm -rf /var/lib/apt/lists/*

# Puppeteer env variable
ENV PUPPETEER_EXECUTABLE_PATH="/usr/bin/google-chrome-stable"

# SDK image for building .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the .csproj
# Make sure your repo folder is named "Nuestra-Senora-del-Rosario"
# and the project file is "Nuestra-Senora-del-Rosario.csproj"
COPY ["Nuestra-Senora-del-Rosario/Nuestra-Senora-del-Rosario.csproj", "Nuestra-Senora-del-Rosario/"]

# Restore packages
RUN dotnet restore "Nuestra-Senora-del-Rosario/Nuestra-Senora-del-Rosario.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/Nuestra-Senora-del-Rosario"
RUN dotnet build "Nuestra-Senora-del-Rosario.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Nuestra-Senora-del-Rosario.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy templates folder if needed
COPY ["Nuestra-Senora-del-Rosario/Templates/", "/app/Templates"]

ENTRYPOINT ["dotnet", "Nuestra-Senora-del-Rosario.dll"]
