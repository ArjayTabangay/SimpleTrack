# Use the official .NET 9.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 9.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["ParcelTracking.Api/ParcelTracking.Api.csproj", "ParcelTracking.Api/"]
COPY ["ParcelTracking.Core/ParcelTracking.Core.csproj", "ParcelTracking.Core/"]
RUN dotnet restore "ParcelTracking.Api/ParcelTracking.Api.csproj"

# Copy source code and build the application
COPY . .
WORKDIR "/src/ParcelTracking.Api"
RUN dotnet build "ParcelTracking.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ParcelTracking.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Install EF Core tools for migrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Create final image
FROM base AS final
WORKDIR /app

# Install PostgreSQL client for connection testing (optional)
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="$PATH:/root/.dotnet/tools"

# Create entrypoint script for running migrations
RUN echo '#!/bin/bash\n\
echo "Starting ParcelTracking API..."\n\
echo "Waiting for database to be ready..."\n\
sleep 10\n\
echo "Running database migrations..."\n\
dotnet ef database update --no-build --verbose || echo "Migration failed, continuing..."\n\
echo "Starting application..."\n\
exec dotnet ParcelTracking.Api.dll' > /app/entrypoint.sh

RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]