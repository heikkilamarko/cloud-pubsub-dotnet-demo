using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using Grpc.Core;

namespace Messaging;

public class PubsubSubscriptionWorker<TMessageHandler>(
    PubsubSubscriptionOptions<TMessageHandler> options,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<PubsubSubscriptionWorker<TMessageHandler>> logger) : BackgroundService where TMessageHandler : class, IPubsubMessageHandler
{
    private SubscriberClient _client;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("start subscription worker: {project}, {topic}, {subscription}", options.Project, options.Topic, options.Subscription);

        if (options.UseEmulator) logger.LogInformation("using pubsub emulator");

        if (options.UseEmulator && options.CreateSubscription)
        {
            logger.LogInformation("create subscription");
            await CreateSubscriptionAsync(stoppingToken);
        }

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

            await Task.Delay(TimeSpan.FromSeconds(options.SubscriptionIntervalSeconds), stoppingToken);
        }

        logger.LogInformation("exit subscription worker");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _client?.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    private Task<SubscriberClient> CreateSubscriberClientAsync(CancellationToken cancellationToken)
    {
        if (options.UseEmulator)
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

    private async Task CreateSubscriptionAsync(CancellationToken cancellationToken)
    {
        var apiClient = await new SubscriberServiceApiClientBuilder
        {
            EmulatorDetection = EmulatorDetection.EmulatorOnly
        }.BuildAsync(cancellationToken);

        try
        {
            var request = new Subscription
            {
                TopicAsTopicName = new TopicName(options.Project, options.Topic),
                SubscriptionName = new SubscriptionName(options.Project, options.Subscription),
                AckDeadlineSeconds = options.AckDeadlineSeconds,
                EnableMessageOrdering = options.EnableMessageOrdering
            };

            await apiClient.CreateSubscriptionAsync(request, cancellationToken);
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            logger.LogInformation("subscription already exists");
        }
    }
}
