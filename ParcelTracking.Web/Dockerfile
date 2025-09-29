# Use the official .NET 9.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 9.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["ParcelTracking.Web/ParcelTracking.Web.csproj", "ParcelTracking.Web/"]
COPY ["ParcelTracking.Core/ParcelTracking.Core.csproj", "ParcelTracking.Core/"]
RUN dotnet restore "ParcelTracking.Web/ParcelTracking.Web.csproj"

# Copy source code and build the application
COPY . .
WORKDIR "/src/ParcelTracking.Web"
RUN dotnet build "ParcelTracking.Web.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ParcelTracking.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create entrypoint script
RUN echo '#!/bin/bash\n\
echo "Starting ParcelTracking Web..."\n\
echo "Waiting for API to be ready..."\n\
sleep 15\n\
echo "Starting Blazor application..."\n\
exec dotnet ParcelTracking.Web.dll' > /app/entrypoint.sh

RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]