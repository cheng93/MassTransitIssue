using System.Reflection;
using System.Threading.Channels;
using MassTransit;
using MassTransitTestHarnessIssue.Worker;

var builder = Host.CreateApplicationBuilder(args);
var channel = Channel.CreateBounded<int>(10);
builder.Services.AddSingleton(channel.Reader);
builder.Services.AddSingleton(channel.Writer);
builder.Services.AddHostedService<Worker>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(Assembly.GetEntryAssembly());
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseRawJsonSerializer(RawSerializerOptions.All, true);
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.Configure<RabbitMqTransportOptions>(
    builder.Configuration.GetSection("MassTransit:Transport")
);

var host = builder.Build();
host.Run();

public partial class Program { }