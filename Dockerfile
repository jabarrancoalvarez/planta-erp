FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files for restore
COPY ["src/PlanTA.SharedKernel/PlanTA.SharedKernel.csproj", "src/PlanTA.SharedKernel/"]
COPY ["src/PlanTA.Seguridad.Domain/PlanTA.Seguridad.Domain.csproj", "src/PlanTA.Seguridad.Domain/"]
COPY ["src/PlanTA.Seguridad.Application/PlanTA.Seguridad.Application.csproj", "src/PlanTA.Seguridad.Application/"]
COPY ["src/PlanTA.Seguridad.Infrastructure/PlanTA.Seguridad.Infrastructure.csproj", "src/PlanTA.Seguridad.Infrastructure/"]
COPY ["src/PlanTA.Inventario.Domain/PlanTA.Inventario.Domain.csproj", "src/PlanTA.Inventario.Domain/"]
COPY ["src/PlanTA.Inventario.Application/PlanTA.Inventario.Application.csproj", "src/PlanTA.Inventario.Application/"]
COPY ["src/PlanTA.Inventario.Infrastructure/PlanTA.Inventario.Infrastructure.csproj", "src/PlanTA.Inventario.Infrastructure/"]
COPY ["src/PlanTA.Produccion.Domain/PlanTA.Produccion.Domain.csproj", "src/PlanTA.Produccion.Domain/"]
COPY ["src/PlanTA.Produccion.Application/PlanTA.Produccion.Application.csproj", "src/PlanTA.Produccion.Application/"]
COPY ["src/PlanTA.Produccion.Infrastructure/PlanTA.Produccion.Infrastructure.csproj", "src/PlanTA.Produccion.Infrastructure/"]
COPY ["src/PlanTA.Compras.Domain/PlanTA.Compras.Domain.csproj", "src/PlanTA.Compras.Domain/"]
COPY ["src/PlanTA.Compras.Application/PlanTA.Compras.Application.csproj", "src/PlanTA.Compras.Application/"]
COPY ["src/PlanTA.Compras.Infrastructure/PlanTA.Compras.Infrastructure.csproj", "src/PlanTA.Compras.Infrastructure/"]
COPY ["src/PlanTA.Ventas.Domain/PlanTA.Ventas.Domain.csproj", "src/PlanTA.Ventas.Domain/"]
COPY ["src/PlanTA.Ventas.Application/PlanTA.Ventas.Application.csproj", "src/PlanTA.Ventas.Application/"]
COPY ["src/PlanTA.Ventas.Infrastructure/PlanTA.Ventas.Infrastructure.csproj", "src/PlanTA.Ventas.Infrastructure/"]
COPY ["src/PlanTA.Calidad.Domain/PlanTA.Calidad.Domain.csproj", "src/PlanTA.Calidad.Domain/"]
COPY ["src/PlanTA.Calidad.Application/PlanTA.Calidad.Application.csproj", "src/PlanTA.Calidad.Application/"]
COPY ["src/PlanTA.Calidad.Infrastructure/PlanTA.Calidad.Infrastructure.csproj", "src/PlanTA.Calidad.Infrastructure/"]
COPY ["src/PlanTA.API/PlanTA.API.csproj", "src/PlanTA.API/"]
COPY ["PlanTA.sln", "./"]
RUN dotnet restore "src/PlanTA.API/PlanTA.API.csproj"

COPY . .
WORKDIR "/src/src/PlanTA.API"
RUN dotnet build "PlanTA.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlanTA.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlanTA.API.dll"]
