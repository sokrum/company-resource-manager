using System.Text;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SyncService.Hubs;

namespace SyncService.Services;

public class QueueConsumerHostedService : BackgroundService
{
    private readonly IHubContext<SyncHub> hub;
    private IConnection? connection;
    private IModel? channel;
    private const string QueueName = "resource-events";

    public QueueConsumerHostedService(IHubContext<SyncHub> hub)
    {
        this.hub = hub;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            await hub.Clients.All.SendAsync("resourceEvent", json, cancellationToken: stoppingToken);
            channel.BasicAck(ea.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        channel?.Dispose();
        connection?.Dispose();
        base.Dispose();
    }
}
