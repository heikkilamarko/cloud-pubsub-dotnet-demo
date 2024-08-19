namespace Demo.Messaging;

public static class PubsubServiceCollectionExtensions
{
    public static void AddPubsubSubscription<TMessageHandler>(this IServiceCollection services, PubsubSubscriptionOptions<TMessageHandler> options)
        where TMessageHandler : class, IPubsubMessageHandler
    {
        services.AddSingleton(options);
        services.AddTransient<TMessageHandler>();
        services.AddHostedService<PubsubSubscriptionWorker<TMessageHandler>>();
    }

    public static void AddPubsubPublisher<T>(this IServiceCollection services, PubsubPublisherOptions<T> options)
    {
        services.AddSingleton(options);
        services.AddSingleton<PubsubPublisher<T>>();
    }
}
