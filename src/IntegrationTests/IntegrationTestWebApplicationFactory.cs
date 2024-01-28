using Domain.Factories;
using Domain.Options;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace IntegrationTests;

public class IntegrationTestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
{
    private readonly IContainer _cassandraContainer = new ContainerBuilder()
        .WithImage("cassandra:5.0")
        .WithName("cassandra-test")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9042))
        .WithPortBinding(9042, 9042)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context,
            configurationBuilder) =>
        {
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(ICassandraSessionFactory));
            services.AddSingleton<ICassandraSessionFactory>(sp =>
                {
                    var options = new CassandraOptions
                    {
                        Address = "127.0.0.1",
                        Port = 9042,
                        KeySpace = "password_storage_system",
                        UserName = "admin",
                        Password = "admin"
                    };
                    var logger = sp.GetRequiredService<ILogger<CassandraSessionFactory>>();
                    return new CassandraSessionFactory(Options.Create(options), logger);
                });
        });

        builder.ConfigureServices(services => { });
    }
    
    public async Task InitializeAsync()
    {
        await _cassandraContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _cassandraContainer.StopAsync();
    }
}