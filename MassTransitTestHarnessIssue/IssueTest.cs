using System.Threading.Channels;
using FluentAssertions;
using MassTransit.Internals;
using MassTransitTestHarnessIssue.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransitTestHarnessIssue;

public class IssueTest : TestBase
{
    [Test]
    public async Task Test()
    {
        var consumer = TestHarness.GetConsumerHarness<TestConsumer>();
        var writer = App.Services.GetRequiredService<ChannelWriter<int>>();

        await writer.WriteAsync(1);
        await writer.WriteAsync(2);
 
        var consumed = await consumer.Consumed
            .SelectAsync(_ => { })
            .ToListAsync();

        Console.WriteLine($"Issue: {string.Join(",", consumed.Select(x => x.MessageObject.As<TestMessage>().Number))}");
        consumed.Should().SatisfyRespectively(x =>
        {
            x.MessageObject.Should().BeOfType<TestMessage>()
                .Which.Number.Should().Be(1);
        },x =>
        {
            x.MessageObject.Should().BeOfType<TestMessage>()
                .Which.Number.Should().Be(2);
        });
    }
}