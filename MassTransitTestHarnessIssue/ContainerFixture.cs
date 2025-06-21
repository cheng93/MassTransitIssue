using Testcontainers.RabbitMq;

[SetUpFixture]
public class ContainerFixture
{
    public static RabbitMqContainer RabbitMq { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        RabbitMq = new RabbitMqBuilder()
            .WithImage("rabbitmq:3.12.13-management-alpine")
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(25672, 15672)
            // .WithReuse(true)
            .Build();

        await RabbitMq.StartAsync();
    }

    [OneTimeTearDown]
    public async ValueTask TearDown()
    {
        await RabbitMq.DisposeAsync();
    }
}