using Google.Api.Gax;
using Google.Cloud.PubSub.V1;

namespace PubSub;

public class PubsubSubscriptionWorker<TMessageHandler>(
    PubsubSubscriptionOptions<TMessageHandler> options,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<PubsubSubscriptionWorker<TMessageHandler>> logger) : BackgroundService where TMessageHandler : class, IPubsubMessageHandler
{
    private SubscriberClient _client;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriptionInterval = TimeSpan.FromSeconds(options.SubscriptionIntervalSeconds);

        while (true)
        {
            try
            {
                logger.LogInformation("start subscription");

                await using (_client = await CreateSubscriberClientAsync(stoppingToken))
                {
                    await _client.StartAsync((message, token) =>
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<TMessageHandler>();
                        return handler.HandleAsync(message, token);
                    });
                }
            }
            catch (Exception err)
            {
                logger.LogError(err, "subscription error");
            }
            finally
            {
                _client = null;
                logger.LogInformation("subscription stopped");
            }

            if (stoppingToken.IsCancellationRequested) break;

            await Task.Delay(subscriptionInterval, stoppingToken);
        }

        logger.LogInformation("exit subscription worker");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _client?.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    protected virtual Task<SubscriberClient> CreateSubscriberClientAsync(CancellationToken cancellationToken)
    {
        if (options.IsEmulator)
        {
            return new SubscriberClientBuilder
            {
                EmulatorDetection = EmulatorDetection.EmulatorOnly,
                SubscriptionName = SubscriptionName.FromProjectSubscription(options.Project, options.Subscription),
                Settings = new SubscriberClient.Settings
                {
                    FlowControlSettings = new FlowControlSettings(options.MaxOutstandingElements, options.MaxOutstandingByteCount)
                }
            }.BuildAsync(cancellationToken);
        }

        return new SubscriberClientBuilder
        {
            SubscriptionName = SubscriptionName.FromProjectSubscription(options.Project, options.Subscription),
            Settings = new SubscriberClient.Settings
            {
                AckExtensionWindow = TimeSpan.FromSeconds(options.AckExtensionWindowSeconds),
                AckDeadline = TimeSpan.FromSeconds(options.AckDeadlineSeconds),
                FlowControlSettings = new FlowControlSettings(options.MaxOutstandingElements, options.MaxOutstandingByteCount)
            }
        }.BuildAsync(cancellationToken);
    }
}
