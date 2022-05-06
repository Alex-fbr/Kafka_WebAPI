using Confluent.Kafka;

namespace KafkaConsumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string topic = "simpletalk_topic";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var conf = new ConsumerConfig
            {
                GroupId = "st_consumer_group",
                BootstrapServers = "kafka:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var builder = new ConsumerBuilder<Ignore, string>(conf).Build();
            builder.Subscribe(topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    var consumer = builder.Consume(stoppingToken);
                    Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
                    _logger.LogInformation($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception)
            {
                builder.Close();
            }
        }
    }
}