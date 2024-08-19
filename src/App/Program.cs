using App.Messaging;
using Messaging;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPubsubSubscriber<Example1MessageHandler>(new(
    Project: "local",
    Topic: "example1",
    Subscription: "example1_sub",
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
    UseEmulator: true));

builder.Services.AddPubsubPublisher<Example2Message>(new(
    Project: "local",
    Topic: "example2",
    UseEmulator: true));

var app = builder.Build();

app.MapPost("/example1", async (PubsubPublisher<Example1Message> publisher, [FromQuery] bool? invalid) =>
{
    var message = invalid == true
        ? new { Id = 1 }.ToJsonPubsubMessage()
        : new Example1Message().ToJsonPubsubMessage();

    await publisher.PublishAsync(message);

    return Results.Ok();
});

app.MapPost("/example2", async (PubsubPublisher<Example2Message> publisher, [FromQuery] bool? invalid) =>
{
    var message = invalid == true
        ? new { Id = 1 }.ToJsonPubsubMessage()
        : new Example2Message().ToJsonPubsubMessage();

    await publisher.PublishAsync(message);

    return Results.Ok();
});

app.Run();
