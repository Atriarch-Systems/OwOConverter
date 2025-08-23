  
# Use Microsoft's official build .NET image.
# https://hub.docker.com/_/microsoft-dotnet-core-sdk/
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

# Accept NuGet credentials as build arguments
ARG NUGET_FEED_URL
ARG NUGET_USER
ARG NUGET_PASS

WORKDIR /

# Install curl and ca-certificates for debugging and SSL
RUN apk add --no-cache curl ca-certificates

COPY ["./src/UwUConverter.csproj", "./src/"]
COPY ["./UwUConverter.Test/UwUConverter.Tests.csproj", "./UwUConverter.Test/"]

# Configure NuGet sources with debugging
RUN if [ ! -z "$NUGET_FEED_URL" ] && [ ! -z "$NUGET_USER" ] && [ ! -z "$NUGET_PASS" ]; then \
        echo "Configuring private NuGet feed..." && \
        echo "Testing connectivity to private feed..." && \
        curl -I "$NUGET_FEED_URL" || echo "Private feed connectivity test failed" && \
        echo "Configuring NuGet sources..." && \
        dotnet nuget remove source nuget.org || true && \
        dotnet nuget add source https://api.nuget.org/v3/index.json --name "nuget.org" && \
        dotnet nuget add source "$NUGET_FEED_URL" --name "AtriarchNuGet" --username "$NUGET_USER" --password "$NUGET_PASS" --store-password-in-clear-text && \
        echo "Listing configured sources:" && \
        dotnet nuget list source; \
    else \
        echo "No private NuGet credentials provided, using public feed only" && \
        dotnet nuget list source; \
    fi

# Try restore with fallback strategy
RUN echo "Attempting restore..." && \
    (dotnet restore "./src/UwUConverter.csproj" --verbosity normal || \
     (echo "Private feed failed, trying with only public feed..." && \
      dotnet nuget remove source AtriarchNuGet || true && \
      dotnet restore "./src/UwUConverter.csproj" --verbosity normal))

RUN echo "Attempting test project restore..." && \
    (dotnet restore "./UwUConverter.Test/UwUConverter.Tests.csproj" --verbosity normal || \
     (echo "Private feed failed, trying with only public feed..." && \
      dotnet nuget remove source AtriarchNuGet || true && \
      dotnet restore "./UwUConverter.Test/UwUConverter.Tests.csproj" --verbosity normal))
COPY . .

# Build stage - this is where tests can be run
RUN dotnet build "./src/UwUConverter.csproj" -c Release --no-restore
RUN dotnet build "./UwUConverter.Test/UwUConverter.Tests.csproj" -c Release --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish "./src/UwUConverter.csproj" -c Release -o out -r alpine-x64 -p:PublishTrimmed=True /p:PublishSingleFile=true

# Use Microsoft's official runtime .NET image.
FROM amd64/alpine:3.14 AS final

# Add some libs required by .NET runtime 
RUN apk add --no-cache libstdc++ libintl

ENV \
    # Configure web servers to bind to port 8080 when present
    ASPNETCORE_URLS=http://+:8080 \
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true \
    # Set the invariant mode since icu-libs isn't included (see https://github.com/dotnet/announcements/issues/20)
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
WORKDIR /app
COPY --from=publish /out .
RUN addgroup -g 1001 -S appuser && adduser -u 1001 -S appuser -G appuser
USER appuser

# Run the web service on container startup.
ENTRYPOINT ["./UwUConverter"]
