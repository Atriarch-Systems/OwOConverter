using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using UwUConverter.StringExtensions;

namespace OwOConverter.StartupHelpers
{
    public class StartupHelpers
    {
        public static void ConfigureApp(WebApplication app, WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.MapGet("/", async context =>
            {
                const string resultString = @"Send a string in the url! Like ""/hello""";
                await context.Response.WriteAsync(resultString.ConvertToUwU());
            });

            app.MapGet("/{text?}", async context =>
            {
                var resultString = "If you see this, your string was bad... Sorry!";
                try
                {
                    var text = context.Request.RouteValues["text"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(text))
                        resultString = text;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}. StackTrace: {ex.StackTrace}");
                }
                finally
                {
                    await context.Response.WriteAsync(resultString.ConvertToUwU());
                }
            });
        }
    }
}
