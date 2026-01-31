using SyncService.Hubs;
using SyncService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<QueuePublisher>();
builder.Services.AddHostedService<QueueConsumerHostedService>();

var app = builder.Build();

app.MapGet("/", () => "SyncService OK");
app.MapHub<SyncHub>("/hubs/sync");

app.Run();
