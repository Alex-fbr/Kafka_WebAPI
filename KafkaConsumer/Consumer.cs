using System.Text.Json;

using Confluent.Kafka;

using Microsoft.Extensions.Options;

namespace KafkaConsumer
{
    public class Consumer : BackgroundService
    {
        private readonly ILogger<Consumer> _logger;
        private readonly IOptions<KafkaOptions> _options;
        private readonly string topic = "topic";

        public Consumer(ILogger<Consumer> logger, IOptions<KafkaOptions> options)
        {
            _logger = logger;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                GroupId = Guid.NewGuid().ToString(),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BootstrapServers = _options.Value.BootstrapServers
            };

            Console.WriteLine($"BootstrapServers = {consumerConfig.BootstrapServers}");
            _logger.LogWarning($"BootstrapServers = {consumerConfig.BootstrapServers}");

            using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            consumer.Subscribe(topic);
            _logger.LogWarning($"Subscribe to topic = {topic}");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    var consumeResult = consumer.Consume(cancellationToken);

                    if (consumeResult != null)
                    {
                        _logger.LogInformation($"Message: {consumeResult.Message.Value}");
                        _logger.LogInformation($"consumeResult = {JsonSerializer.Serialize(consumeResult)}");
                    }
                    else
                    {
                        _logger.LogWarning("Empty...");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while get consume. Message {ex.Message}");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}