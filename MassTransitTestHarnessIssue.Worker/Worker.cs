using System.Threading.Channels;
using MassTransit;

namespace MassTransitTestHarnessIssue.Worker;

public class Worker(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var endpointProvider = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
        var reader = scope.ServiceProvider.GetRequiredService<ChannelReader<int>>();
        await foreach (var number in reader.ReadAllAsync(stoppingToken))
        {
            var endpoint = await endpointProvider.GetSendEndpoint(new Uri("exchange:foo"));

            Console.WriteLine($"Writing {number}");
            await endpoint.Send(new TestMessage(number), stoppingToken);
        }
    }
}