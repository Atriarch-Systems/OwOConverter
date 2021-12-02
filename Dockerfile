  
# Use Microsoft's official build .NET image.
# https://hub.docker.com/_/microsoft-dotnet-core-sdk/
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /app

# Install production dependencies.
# Copy csproj and restore as distinct layers.
COPY *.csproj ./
RUN dotnet restore "OwOConverter.csproj"

# Copy local code to the container image.
COPY . ./
WORKDIR /app

# Build a release artifact.
RUN dotnet publish "OwOConverter.csproj" -c Release -o out -r alpine-x64 -p:PublishTrimmed=True /p:PublishSingleFile=true

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
  DOTNET_RUNNING_IN_CONTAINER=true

# Use Microsoft's official runtime .NET image.
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine AS base
WORKDIR /app
COPY --from=build /app/out .
RUN addgroup -g 1001 -S appuser && adduser -u 1001 -S appuser -G appuser
USER appuser

# Run the web service on container startup.
ENTRYPOINT ["./OwOConverter"]
