using Microsoft.AspNetCore.Builder;
using UwUConverter.StartupHelpers;

var app = WebApplication.CreateBuilder(args).Build();

StartupHelpers.ConfigureApp(app);

await app.RunAsync();