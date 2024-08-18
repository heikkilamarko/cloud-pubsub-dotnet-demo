namespace PubSub;

public static class PubSubServiceCollectionExtensions
{
    public static void AddPubSubSubscription<TMessageHandler>(this IServiceCollection services, PubsubSubscriptionOptions<TMessageHandler> options)
        where TMessageHandler : class, IPubsubMessageHandler
    {
        services.AddSingleton(options);
        services.AddTransient<TMessageHandler>();
        services.AddHostedService<PubsubSubscriptionWorker<TMessageHandler>>();
    }
}
