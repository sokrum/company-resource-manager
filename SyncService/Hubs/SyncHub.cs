using Microsoft.AspNetCore.SignalR;
using SyncService.Services;

namespace SyncService.Hubs;

public class SyncHub : Hub
{
    private readonly QueuePublisher publisher;

    public SyncHub(QueuePublisher publisher)
    {
        this.publisher = publisher;
    }

    public Task Publish(string jsonEvent)
    {
        return publisher.PublishAsync(jsonEvent);
    }
}