  
# Use Microsoft's official build .NET image.
# https://hub.docker.com/_/microsoft-dotnet-core-sdk/
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["src/UwUConverter.csproj", "."]
COPY ["UwUConverter.Tests/UwUConverter.Tests.csproj", "."]
RUN dotnet restore "./UwUConverter.csproj"
COPY . .
#WORKDIR "/src/."
RUN dotnet publish "UwUConverter.csproj" -c Release -o out -r alpine-x64 -p:PublishTrimmed=True /p:PublishSingleFile=true

# Use Microsoft's official runtime .NET image.
FROM amd64/alpine:3.14

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
COPY --from=build /app/out .
RUN addgroup -g 1001 -S appuser && adduser -u 1001 -S appuser -G appuser
USER appuser

# Run the web service on container startup.
ENTRYPOINT ["./UwUConverter"]
