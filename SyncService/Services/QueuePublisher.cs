using System.Text;

namespace SyncService.Services;

public class QueuePublisher : IDisposable
{
    private readonly RabbitMQ.Client.IConnection connection;
    private readonly RabbitMQ.Client.IModel channel;
    private const string QueueName = "resource-events";

    public QueuePublisher()
    {
        var factory = new RabbitMQ.Client.ConnectionFactory { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public Task PublishAsync(string json)
    {
        var body = Encoding.UTF8.GetBytes(json);

        var props = channel.CreateBasicProperties();
        props.Persistent = true;

        channel.BasicPublish(
            exchange: "",
            routingKey: QueueName,
            mandatory: false,
            basicProperties: props,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        channel.Dispose();
        connection.Dispose();
    }
}
