using MassTransit;
using MassTransitTestHarnessIssue.Worker;

namespace MassTransitTestHarnessIssue;

public class TestConsumer : IConsumer<TestMessage>
{
    public Task Consume(ConsumeContext<TestMessage> context)
        => Task.CompletedTask;

    public class Definition : ConsumerDefinition<TestConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TestConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbit)
            {
                rabbit.Bind("foo");
            }
        }
    }
}