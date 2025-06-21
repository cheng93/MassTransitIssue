using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MassTransitTestHarnessIssue;

[TestFixture]
public abstract class TestBase
{
    protected WebApplicationFactory<Program> App { get; private set; } = null!;

    protected ITestHarness TestHarness { get; private set; } = null!;

    [SetUp]
    public void SetUpBase()
    {
        App = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.Configure(_ => { });
                    builder.ConfigureServices(services =>
                    {
                        services.AddMassTransitTestHarness(x =>
                        {
                            x.AddConsumersFromNamespaceContaining<TestConsumer>();
                            x.UsingRabbitMq((context, cfg) =>
                            {
                                cfg.UseRawJsonSerializer(RawSerializerOptions.All, true);
                                cfg.ConfigureEndpoints(context);
                            });
                            x.SetTestTimeouts(TimeSpan.FromSeconds(1.5),  TimeSpan.FromSeconds(5));
                        });
                    });
                    builder.UseSetting("MassTransit:Transport:Host", ContainerFixture.RabbitMq.Hostname);
                    builder.UseSetting("MassTransit:Transport:Port", ContainerFixture.RabbitMq.GetMappedPublicPort(5672).ToString());
                }
            );
        TestHarness = App.Services.GetTestHarness();
    }

    [TearDown]
    public async Task TearDownBase()
    {
        await App.DisposeAsync();
    }
}