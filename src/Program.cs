using Microsoft.AspNetCore.Builder;
using UwUConverter.StartupHelpers;

// Set the port to 8080 for consistency between local and container environments
Environment.SetEnvironmentVariable("ASPNETCORE_HTTP_PORTS", "8080");

var app = WebApplication.CreateBuilder(args).Build();

StartupHelpers.ConfigureApp(app);

await app.RunAsync();