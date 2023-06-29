using Xunit;
using UwUConverter.StringExtensions;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using OwOConverter.StartupHelpers;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace UwUConverter.Tests
{
    public class UwUConverterTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UwUConverterTests(WebApplicationFactory<Program> factory)
        {
            // Arrange
            _client = factory.CreateClient();
        }

        [Fact]
        public void TestUwUConversion()
        {
            const string input = "Hello, world!";
            const string expected = "Hewwo, wowwd!";

            var actual = input.ConvertToUwU();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task TestRootEndpoint()
        {
            // Act
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            const string expected = @"Send a string in the url! Like ""/hello""";
            Assert.Equal(expected.ConvertToUwU(), responseString);
        }

        [Fact]
        public async Task TestVariableEndpoint()
        {
            // Act
            var response = await _client.GetAsync("/hello");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            const string expected = "hello";
            Assert.Equal(expected.ConvertToUwU(), responseString);
        }
    }
}