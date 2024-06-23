# Dockerfile
# Define a imagem base (SDK do .NET Core 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER app
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src

COPY ["SensorizMonitoring.csproj", "SensorizMonitoring/"]
RUN dotnet restore "./SensorizMonitoring/SensorizMonitoring.csproj"

COPY . .
WORKDIR "/src/SensorizMonitoring"
RUN dotnet build "./SensorizMonitoring.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./SensorizMonitoring.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SensorizMonitoring.dll"]