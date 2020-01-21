# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

# Copy source and build app (this will also perform a restore)
WORKDIR /source
COPY . .
RUN dotnet publish -c release -o /app 

# Final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "DotNetCoreSqlDb.dll"]