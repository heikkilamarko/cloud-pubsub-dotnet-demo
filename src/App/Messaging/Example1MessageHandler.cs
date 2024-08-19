using Google.Cloud.PubSub.V1;
using Messaging;

namespace App.Messaging;

public class Example1MessageHandler(ILogger<Example1MessageHandler> logger) : IPubsubMessageHandler
{
    public Task<SubscriberClient.Reply> HandleAsync(PubsubMessage message, CancellationToken cancellationToken)
    {
        Example1Message messageData;
        try
        {
            messageData = message.JsonDeserialize<Example1Message>();
        }
        catch (Exception err)
        {
            logger.LogWarning(err, "[{message_id}] invalid message format. drop or save as dead letter.", message.MessageId);
            return Task.FromResult(SubscriberClient.Reply.Ack);
        }

        logger.LogInformation("[{message_id}] {id} {name}", message.MessageId, messageData.Id, messageData.Name);
        return Task.FromResult(SubscriberClient.Reply.Ack);
    }
}
