using Microsoft.AspNetCore.Builder;
using OwOConverter.StartupHelpers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

StartupHelpers.ConfigureApp(app, builder);

await app.RunAsync();