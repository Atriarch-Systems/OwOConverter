  
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

# Configure ONLY the private Nexus feed (which proxies public packages)
RUN if [ ! -z "$NUGET_FEED_URL" ] && [ ! -z "$NUGET_USER" ] && [ ! -z "$NUGET_PASS" ]; then \
        echo "Configuring private Nexus feed as the ONLY source (it proxies public packages)..." && \
        echo "Testing authenticated connectivity to Nexus..." && \
        curl -I -u "$NUGET_USER:$NUGET_PASS" "$NUGET_FEED_URL" && \
        echo "Removing all default sources..." && \
        dotnet nuget remove source nuget.org || true && \
        echo "Adding ONLY the private Nexus feed..." && \
        dotnet nuget add source "$NUGET_FEED_URL" --name "AtriarchNexus" --username "$NUGET_USER" --password "$NUGET_PASS" --store-password-in-clear-text && \
        echo "Creating NuGet.config with timeout settings..." && \
        echo '<?xml version="1.0" encoding="utf-8"?>' > /tmp/nuget.config && \
        echo '<configuration>' >> /tmp/nuget.config && \
        echo '  <config>' >> /tmp/nuget.config && \
        echo '    <add key="http.timeout" value="300000" />' >> /tmp/nuget.config && \
        echo '    <add key="http.retrycount" value="3" />' >> /tmp/nuget.config && \
        echo '  </config>' >> /tmp/nuget.config && \
        echo '  <packageSources>' >> /tmp/nuget.config && \
        echo "    <add key=\"AtriarchNexus\" value=\"$NUGET_FEED_URL\" />" >> /tmp/nuget.config && \
        echo '  </packageSources>' >> /tmp/nuget.config && \
        echo '  <packageSourceCredentials>' >> /tmp/nuget.config && \
        echo '    <AtriarchNexus>' >> /tmp/nuget.config && \
        echo "      <add key=\"Username\" value=\"$NUGET_USER\" />" >> /tmp/nuget.config && \
        echo "      <add key=\"ClearTextPassword\" value=\"$NUGET_PASS\" />" >> /tmp/nuget.config && \
        echo '    </AtriarchNexus>' >> /tmp/nuget.config && \
        echo '  </packageSourceCredentials>' >> /tmp/nuget.config && \
        echo '</configuration>' >> /tmp/nuget.config && \
        echo "NuGet.config created:" && \
        cat /tmp/nuget.config && \
        echo "Configured sources:" && \
        dotnet nuget list source; \
    else \
        echo "ERROR: Private NuGet credentials are required!" && \
        exit 1; \
    fi

# Restore using ONLY the private Nexus feed with timeout settings
RUN echo "Restoring packages using private Nexus feed (proxying public packages)..." && \
    NUGET_HTTP_CACHE_PATH=/tmp/.nuget/v3-cache \
    dotnet restore "./src/UwUConverter.csproj" \
        --verbosity normal \
        --runtime alpine-x64 \
        --disable-parallel \
        --configfile /tmp/nuget.config

RUN echo "Restoring test project packages..." && \
    NUGET_HTTP_CACHE_PATH=/tmp/.nuget/v3-cache \
    dotnet restore "./UwUConverter.Test/UwUConverter.Tests.csproj" \
        --verbosity normal \
        --runtime alpine-x64 \
        --disable-parallel \
        --configfile /tmp/nuget.config
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
