using Google.Cloud.PubSub.V1;
using Messaging;

namespace MessageHandlers;

public class Example2Handler(ILogger<Example2Handler> logger) : IPubsubMessageHandler
{
    public Task<SubscriberClient.Reply> HandleAsync(PubsubMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation("message: [{publishTime}] [{messageId}]: {data}", message.PublishTime.ToDateTimeOffset().ToString("o"), message.MessageId, message.Data.ToStringUtf8());
        return Task.FromResult(SubscriberClient.Reply.Ack);
    }
}
