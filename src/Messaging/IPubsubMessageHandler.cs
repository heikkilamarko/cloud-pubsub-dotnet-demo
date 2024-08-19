using Google.Cloud.PubSub.V1;

namespace Demo.Messaging;

public interface IPubsubMessageHandler
{
    Task<SubscriberClient.Reply> HandleAsync(PubsubMessage message, CancellationToken cancellationToken);
}
