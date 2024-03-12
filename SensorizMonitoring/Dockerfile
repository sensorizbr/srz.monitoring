# Use a imagem base do .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copie o projeto e restaure as dependências
COPY ./App/*.csproj ./
RUN dotnet restore

# Copie o código-fonte e compile o aplicativo
COPY ./App ./
RUN dotnet publish -c Release -o out

# Crie a imagem final
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build-env /app/out ./

# Execute o aplicativo
ENTRYPOINT ["dotnet", "SensorizMonitoring.dll"]