using Google.Api.Gax;
using Google.Cloud.PubSub.V1;

namespace Messaging;

public sealed class PubsubPublisher<T> : IAsyncDisposable
{
    private readonly Task _initializationTask = null;
    private readonly PubsubPublisherOptions<T> _options;

    private PublisherClient _client;

    public PubsubPublisher(PubsubPublisherOptions<T> options)
    {
        _options = options;
        _initializationTask = InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_client != null) await _client.DisposeAsync();
    }

    public async Task PublishAsync(PubsubMessage message)
    {
        await _initializationTask;
        await _client.PublishAsync(message);
    }

    private async Task InitializeAsync()
    {
        _client = await CreatePublisherClientAsync();
    }

    private Task<PublisherClient> CreatePublisherClientAsync(CancellationToken cancellationToken = default)
    {
        if (_options.UseEmulator)
        {
            return new PublisherClientBuilder
            {
                EmulatorDetection = EmulatorDetection.EmulatorOnly,
                TopicName = new TopicName(_options.Project, _options.Topic)

            }.BuildAsync(cancellationToken);
        }

        return new PublisherClientBuilder
        {
            TopicName = new TopicName(_options.Project, _options.Topic)
        }.BuildAsync(cancellationToken);
    }
}
