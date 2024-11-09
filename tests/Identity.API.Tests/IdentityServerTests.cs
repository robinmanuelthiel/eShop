using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using Duende.IdentityServer.Services;
using System.Security.Claims;
using IdentityModel;
using eShop.Identity.API;
using eShop.Identity.API.Configuration;
using eShop.Identity.API.Models;
using eShop.Identity.API.Services;
using eShop.Identity.API.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace Identity.API.Tests
{
    public class TestServerFixture : IDisposable
    {
        public TestServer Server { get; }
        public HttpClient Client { get; }
        public IConfiguration Configuration { get; }

        public TestServerFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["IdentityUrl"] = "http://localhost:5000",
                    ["BasketApiClient"] = "basketswaggerui",
                    ["BasketApiSecret"] = "secret",
                    ["ConnectionString"] = "InMemoryDb"
                })
                .Build();

            Server = new TestServer(new WebHostBuilder()
                .UseConfiguration(Configuration)
                .UseStartup<TestStartup>());

            Client = Server.CreateClient();
        }

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }
    }

    public class IdentityServerTests : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _fixture;

        public IdentityServerTests(TestServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task DiscoveryEndpoint_ReturnsValidResponse()
        {
            // Act
            var response = await _fixture.Client.GetAsync(".well-known/openid-configuration");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(content);
            Assert.True(document.RootElement.TryGetProperty("issuer", out _));
            Assert.True(document.RootElement.TryGetProperty("authorization_endpoint", out _));
            Assert.True(document.RootElement.TryGetProperty("token_endpoint", out _));
        }

        [Fact]
        public async Task TokenEndpoint_WithClientCredentials_ReturnsValidToken()
        {
            // Arrange
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", "basketswaggerui"),
                new KeyValuePair<string, string>("client_secret", "secret"),
                new KeyValuePair<string, string>("scope", "basket")
            });

            // Act
            var response = await _fixture.Client.PostAsync("connect/token", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var tokenResponse = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(tokenResponse);
            Assert.True(document.RootElement.TryGetProperty("access_token", out _));
            Assert.True(document.RootElement.TryGetProperty("expires_in", out _));
            Assert.Equal("Bearer", document.RootElement.GetProperty("token_type").GetString());
        }
    }
}
