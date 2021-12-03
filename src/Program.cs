using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using OwOConverter.StringExtensions;
using System.Threading.Tasks;

namespace OwOConverter
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var app = builder.Build();

            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    const string resultString = @"Send a string in the url! Like ""/hello""";
                    await context.Response.WriteAsync(resultString.ConvertToOwO());
                });
                endpoints.MapGet("/{*text}", async context =>
                {
                    var resultString = "If you see this, your string was bad... Sorry!";
                    try
                    {
                        var inputString = context.GetRouteValue("text").ToString();
                        if (!string.IsNullOrWhiteSpace(inputString))
                            resultString = inputString;
                    }
                    catch
                    { }
                    finally
                    {
                        await context.Response.WriteAsync(resultString.ConvertToOwO());
                    }
                });
            });

            await app.RunAsync();
        }
    }
}
