# Use Microsoft's official build .NET image.
# https://hub.docker.com/_/microsoft-dotnet-core-sdk/
FROM mcr.microsoft.com/dotnet/core/sdk:6.0 AS build
WORKDIR /app

# Build arguments for private NuGet feed
ARG NUGET_FEED_URL
ARG NUGET_USER
ARG NUGET_PASS

# Configure NuGet for private feed if provided
RUN if [ ! -z "$NUGET_FEED_URL" ] && [ ! -z "$NUGET_USER" ] && [ ! -z "$NUGET_PASS" ]; then \
        dotnet nuget add source "$NUGET_FEED_URL" --name "PrivateFeed" --username "$NUGET_USER" --password "$NUGET_PASS" --store-password-in-clear-text; \
    fi

# Copy project files and restore dependencies
COPY src/*.csproj ./
RUN dotnet restore "UwUConverter.csproj"

# Copy source code and build
COPY src/ ./
WORKDIR /app

# Build a release artifact
RUN dotnet publish "UwUConverter.csproj" -c Release -o out

# Use Microsoft's official runtime .NET image.
# https://hub.docker.com/_/microsoft-dotnet-core-aspnet/
FROM mcr.microsoft.com/dotnet/core/aspnet:6.0 AS base
WORKDIR /app
COPY --from=build /app/out ./
RUN addgroup -g 1001 -S appuser && adduser -u 1001 -S appuser -G appuser
USER appuser

# Run the web service on container startup.
ENTRYPOINT ["dotnet", "UwUConverter.dll"]