# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["PermissionManager.sln", "./"]
COPY ["PermissionManager.API/PermissionManager.API.csproj", "PermissionManager.API/"]
COPY ["PermissionManager.Application/PermissionManager.Application.csproj", "PermissionManager.Application/"]
COPY ["PermissionManager.Domain/PermissionManager.Domain.csproj", "PermissionManager.Domain/"]
COPY ["PermissionManager.Persistence/PermissionManager.Persistence.csproj", "PermissionManager.Persistence/"]
COPY ["PermissionManager.Infrastructure/PermissionManager.Infrastructure.csproj", "PermissionManager.Infrastructure/"]
COPY ["PermissionManager.Core/PermissionManager.Core.csproj", "PermissionManager.Core/"]
COPY ["PermissionManager.Test/PermissionManager.Test.csproj", "PermissionManager.Test/"]
COPY ["PermissionManager.IntegrationTest/PermissionManager.IntegrationTest.csproj", "PermissionManager.IntegrationTest/"]
RUN dotnet restore

# Copy everything else and publish
COPY . .
WORKDIR /src/PermissionManager.API
RUN dotnet publish -c Release -o /app --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "PermissionManager.API.dll"]
