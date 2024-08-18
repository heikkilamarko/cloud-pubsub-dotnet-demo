using Handlers;
using PubSub;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPubSubSubscription<Example1Handler>(new(
    Project: "local",
    Subscription: "example1_sub",
    IsEmulator: true));

builder.Services.AddPubSubSubscription<Example2Handler>(new(
    Project: "local",
    Subscription: "example2_sub",
    IsEmulator: true));

var app = builder.Build();

app.Run();
