using Google.Cloud.PubSub.V1;

namespace PubSub;

public interface IPubsubMessageHandler
{
    Task<SubscriberClient.Reply> HandleAsync(PubsubMessage message, CancellationToken cancellationToken);
}
