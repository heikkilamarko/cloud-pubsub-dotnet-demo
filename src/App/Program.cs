using App.Messaging;
using Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPubsubSubscriber<Example1MessageHandler>(new(
    Project: "local",
    Topic: "example1",
    Subscription: "example1_sub",
    DeadLetterTopic: "example1_dl",
    UseEmulator: true,
    CreateSubscription: true));

builder.Services.AddPubsubSubscriber<Example2MessageHandler>(new(
    Project: "local",
    Topic: "example2",
    Subscription: "example2_sub",
    UseEmulator: true,
    CreateSubscription: true));

builder.Services.AddPubsubPublisher<Example1Message>(new(
    Project: "local",
    Topic: "example1",
    UseEmulator: true,
    CreateTopic: true));

builder.Services.AddPubsubPublisher<Example2Message>(new(
    Project: "local",
    Topic: "example2",
    UseEmulator: true,
    CreateTopic: true));

var app = builder.Build();

app.MapPost("/publish/example1", async (object body, PubsubPublisher<Example1Message> publisher) =>
{
    await publisher.PublishAsync(body.ToJsonPubsubMessage());
    return Results.Ok();
});

app.MapPost("/publish/example2", async (object body, PubsubPublisher<Example2Message> publisher) =>
{
    await publisher.PublishAsync(body.ToJsonPubsubMessage());
    return Results.Ok();
});

app.Run();
