using Google.Cloud.PubSub.V1;
using Messaging;

namespace App.Messaging;

public class Example2MessageHandler(ILogger<Example2MessageHandler> logger) : IPubsubMessageHandler
{
    public Task<SubscriberClient.Reply> HandleAsync(PubsubMessage message, CancellationToken cancellationToken)
    {
        Example2Message messageData;
        try
        {
            messageData = message.JsonDeserialize<Example2Message>();
        }
        catch (Exception err)
        {
            logger.LogWarning(err, "[{message_id}] invalid message", message.MessageId);
            return Task.FromResult(SubscriberClient.Reply.Nack);
        }

        logger.LogInformation("[{message_id}] {id} {name}", message.MessageId, messageData.Id, messageData.Name);
        return Task.FromResult(SubscriberClient.Reply.Ack);
    }
}
