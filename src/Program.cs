using Microsoft.AspNetCore.Builder;
using UwUConverter.StartupHelpers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

StartupHelpers.ConfigureApp(app, builder);

await app.RunAsync();

// Make the implicit Program class public so test projects can access it
// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program { }