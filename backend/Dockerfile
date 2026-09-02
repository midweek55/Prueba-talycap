# =============================================
# Etapa 1: build - compila la solucion con el SDK de .NET
# =============================================
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copia primero los .csproj para aprovechar la cache de capas de Docker en el restore
COPY ApiClientes.Models/ApiClientes.Models.csproj ApiClientes.Models/
COPY ApiClientes.DTOs/ApiClientes.DTOs.csproj ApiClientes.DTOs/
COPY ApiClientes.Repositories/ApiClientes.Repositories.csproj ApiClientes.Repositories/
COPY ApiClientes.Services/ApiClientes.Services.csproj ApiClientes.Services/
COPY ApiClientes.API/ApiClientes.API.csproj ApiClientes.API/

RUN dotnet restore ApiClientes.API/ApiClientes.API.csproj

# Copia el resto del codigo fuente y publica
COPY . .
RUN dotnet publish ApiClientes.API/ApiClientes.API.csproj -c Release -o /app/publish --no-restore

# =============================================
# Etapa 2: runtime - imagen ligera solo con el runtime de ASP.NET
# =============================================
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ApiClientes.API.dll"]
