# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY Steamerfy.Server/*.csproj ./Steamerfy.Server/

# Restore dependencies
RUN dotnet restore Steamerfy.Server/Steamerfy.Server.csproj

# Copy the rest of the source code
COPY Steamerfy.Server/. ./Steamerfy.Server/

# Build and publish
WORKDIR /src/Steamerfy.Server
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Configure environment
ENV ASPNETCORE_URLS=http://+:5063
EXPOSE 5063

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5063/health || exit 1

ENTRYPOINT ["dotnet", "Steamerfy.Server.dll"]