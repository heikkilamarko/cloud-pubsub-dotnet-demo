namespace PubSub;

public record PubsubSubscriptionOptions<T>(
    string Project,
    string Subscription,
    int AckExtensionWindowSeconds = 4,
    int AckDeadlineSeconds = 10,
    int SubscriptionIntervalSeconds = 10,
    long MaxOutstandingElements = 5,
    long MaxOutstandingByteCount = 10240,
    bool EnableMessageOrdering = false,
    bool IsEmulator = false
);
