using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using S14_ProjetSession.Data;
using System.Net;
using Xunit;

namespace S14_ProjetSessionTests.Integration.ResidenceTests;

public class ResidenceRoutageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ResidenceRoutageTests(WebApplicationFactory<Program> factory)
    {
        var mockRepo = new MockResidenceRepository();

       _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IResidenceRepository>(mockRepo);
            });
            builder.UseEnvironment("Test");
        });

        _client = _factory.CreateClient();
    }

    [InlineData("/Residence")]
    [InlineData("/Residence/ResidenceDetails/1")]
    [Theory]
    public async Task RouteExiste(string url)
    {
        HttpResponseMessage response = await _client.GetAsync(url);
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrEmpty(body));
    }

    [InlineData("/Residence/InvalidRoute")]
    [Theory]
    public async Task RouteNexistePas(string url)
    {
        HttpResponseMessage response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}