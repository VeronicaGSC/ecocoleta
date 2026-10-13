# syntax=docker/dockerfile:1
# =====================================================================
# EcoColeta API — Dockerfile multi-stage
#   Estágio 1 (build):   SDK do .NET compila e publica a aplicação
#   Estágio 2 (runtime): imagem enxuta só com o runtime ASP.NET
# =====================================================================

# ---------- Estágio 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copia apenas o .csproj primeiro para aproveitar o cache de camadas do restore
COPY src/EcoColeta.Api/EcoColeta.Api.csproj src/EcoColeta.Api/
RUN dotnet restore src/EcoColeta.Api/EcoColeta.Api.csproj

# Copia o restante do código e publica em modo Release
COPY src/ src/
RUN dotnet publish src/EcoColeta.Api/EcoColeta.Api.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Estágio 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Versão da imagem (a pipeline injeta o SHA do commit)
ARG APP_VERSION=local
ENV APP_VERSION=${APP_VERSION} \
    ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

# curl é usado apenas pelo HEALTHCHECK
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Segurança: executa com usuário não-root (já existente na imagem oficial)
USER $APP_UID

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
    CMD curl -fs http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "EcoColeta.Api.dll"]
