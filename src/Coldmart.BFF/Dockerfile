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

# Estágio de restore: copia apenas os .csproj para aproveitar o cache de camadas
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src
COPY ["src/Coldmart.Core/Coldmart.Core.csproj", "Coldmart.Core/"]
COPY ["src/Coldmart.Core.Data/Coldmart.Core.Data.csproj", "Coldmart.Core.Data/"]
COPY ["src/Coldmart.Auth.Data/Coldmart.Auth.Data.csproj", "Coldmart.Auth.Data/"]
COPY ["src/Coldmart.Pagamentos.Domain/Coldmart.Pagamentos.Domain.csproj", "Coldmart.Pagamentos.Domain/"]
COPY ["src/Coldmart.Pagamentos.Data/Coldmart.Pagamentos.Data.csproj", "Coldmart.Pagamentos.Data/"]
COPY ["src/Coldmart.Pagamentos.Business/Coldmart.Pagamentos.Business.csproj", "Coldmart.Pagamentos.Business/"]
COPY ["src/Coldmart.Pagamentos.API/Coldmart.Pagamentos.API.csproj", "Coldmart.Pagamentos.API/"]
COPY ["src/Coldmart.BFF/Coldmart.BFF.csproj", "Coldmart.BFF/"]
RUN dotnet restore "Coldmart.BFF/Coldmart.BFF.csproj"

# Estágio de publish: compila e publica em uma única etapa
FROM restore AS publish
ARG BUILD_CONFIGURATION=Release
COPY . .
WORKDIR "/src/Coldmart.BFF"
RUN dotnet publish "./Coldmart.BFF.csproj" -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false

# Estágio final: imagem de produção mínima
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
HEALTHCHECK --interval=10s --timeout=5s --start-period=20s --retries=3 CMD curl -f http://localhost:8080/healthz || exit 1
ENTRYPOINT ["dotnet", "Coldmart.BFF.dll"]
