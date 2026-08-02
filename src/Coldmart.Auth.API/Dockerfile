# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Estágio base: runtime enxuto com curl para health check
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true

# Estágio de restore: copia o repositório e restaura o projeto da raiz do contexto de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src
COPY . .
RUN dotnet restore "./src/Coldmart.Auth.API/Coldmart.Auth.API.csproj"

# Estágio de publish: compila e publica em uma única etapa
FROM restore AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src"
RUN dotnet publish "./src/Coldmart.Auth.API/Coldmart.Auth.API.csproj" -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false

# Estágio final: imagem de produção mínima
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
HEALTHCHECK --interval=10s --timeout=5s --start-period=20s --retries=3 CMD curl -f http://localhost:8080/healthz || exit 1
ENTRYPOINT ["dotnet", "Coldmart.Auth.API.dll"]