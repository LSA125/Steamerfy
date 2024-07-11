# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory
WORKDIR /src

# Copy the solution file and restore dependencies
COPY *.sln .
COPY Steamerfy.Server/*.csproj ./Steamerfy.Server/
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Set environment variables for SSL certificate and key paths
ENV ASPNETCORE_URLS=https://+:5063
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=your-password
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.aspnet/https/your-app.pfx

# Copy the SSL certificate and key files
COPY $APPDATA/ASP.NET/https/your-app.pfx /root/.aspnet/https/your-app.pfx

# Expose port 5063 for SSL
EXPOSE 5063

# Start the application
ENTRYPOINT ["dotnet", "Steamerfy.Server.dll"]