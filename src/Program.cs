using Handlers;
using PubSub;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPubSubSubscription<Example1Handler>(new(
    Project: "local",
    Topic: "example1",
    Subscription: "example1_sub",
    UseEmulator: true,
    CreateSubscription: true));

builder.Services.AddPubSubSubscription<Example2Handler>(new(
    Project: "local",
    Topic: "example2",
    Subscription: "example2_sub",
    UseEmulator: true,
    CreateSubscription: true));

var app = builder.Build();

app.Run();
