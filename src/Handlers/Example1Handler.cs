using Google.Cloud.PubSub.V1;
using PubSub;

namespace Handlers;

public class Example1Handler(ILogger<Example1Handler> logger) : IPubsubMessageHandler
{
    public Task<SubscriberClient.Reply> HandleAsync(PubsubMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation("message: [{publishTime}] [{messageId}]: {data}", message.PublishTime.ToDateTimeOffset().ToString("o"), message.MessageId, message.Data.ToStringUtf8());
        return Task.FromResult(SubscriberClient.Reply.Ack);
    }
}
