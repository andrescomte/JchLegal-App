# Imagen base runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Configurar puerto interno
ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

# Imagen de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar proyectos para restaurar dependencias
COPY ["JchLegal.ApplicationApi/JchLegal.ApplicationApi.csproj", "JchLegal.ApplicationApi/"]
COPY ["JchLegal.Domain/JchLegal.Domain.csproj", "JchLegal.Domain/"]
COPY ["JchLegal.Infrastructure/JchLegal.Infrastructure.csproj", "JchLegal.Infrastructure/"]

RUN dotnet restore "JchLegal.ApplicationApi/JchLegal.ApplicationApi.csproj"

# Copiar el resto del código
COPY . .

WORKDIR "/src/JchLegal.ApplicationApi"
RUN dotnet build "JchLegal.ApplicationApi.csproj" -c Release -o /app/build

# Publicar
FROM build AS publish
RUN dotnet publish "JchLegal.ApplicationApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "JchLegal.ApplicationApi.dll"]