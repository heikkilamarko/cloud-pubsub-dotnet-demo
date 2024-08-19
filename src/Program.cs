using Demo;
using Demo.MessageHandlers;
using Demo.Messaging;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPubsubSubscription<Example1Handler>(new(
    Project: "local",
    Topic: "example1",
    Subscription: "example1_sub",
    UseEmulator: true,
    CreateSubscription: true));

builder.Services.AddPubsubSubscription<Example2Handler>(new(
    Project: "local",
    Topic: "example2",
    Subscription: "example2_sub",
    UseEmulator: true,
    CreateSubscription: true));

builder.Services.AddPubsubPublisher<IExample1Publisher>(new(
    Project: "local",
    Topic: "example1",
    UseEmulator: true));

builder.Services.AddPubsubPublisher<IExample2Publisher>(new(
    Project: "local",
    Topic: "example2",
    UseEmulator: true));

var app = builder.Build();

app.MapPost("/example1", async (PubsubPublisher<IExample1Publisher> publisher, CancellationToken cancellationToken) =>
{
    await publisher.PublishAsync(new PubsubMessage
    {
        Data = ByteString.CopyFromUtf8("example1")
    });

    return Results.Ok();
});

app.MapPost("/example2", async (PubsubPublisher<IExample2Publisher> publisher, CancellationToken cancellationToken) =>
{
    await publisher.PublishAsync(new PubsubMessage
    {
        Data = ByteString.CopyFromUtf8("example2")
    });

    return Results.Ok();
});

app.Run();

namespace Demo
{
    public interface IExample1Publisher { }
    public interface IExample2Publisher { }
}
