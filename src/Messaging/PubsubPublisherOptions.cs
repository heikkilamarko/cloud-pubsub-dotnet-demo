namespace Demo.Messaging;

public record PubsubPublisherOptions<T>(
    string Project,
    string Topic,
    bool UseEmulator = false,
    bool CreateTopic = false
);
