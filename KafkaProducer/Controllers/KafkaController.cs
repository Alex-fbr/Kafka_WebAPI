using Confluent.Kafka;

using KafkaProducer.Settings;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KafkaProducer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KafkaController : ControllerBase
    {
        private readonly ILogger<KafkaController> _logger;
        private readonly KafkaOptions _options;
        private readonly string topic = "topic";
        private readonly ProducerConfig _producerConfig;
        public KafkaController(ILogger<KafkaController> logger, IOptions<KafkaOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _producerConfig = new()
            {
                BootstrapServers = _options.BootstrapServers
            };
        }

        [HttpPost("/send")]
        public IActionResult Send([FromQuery] string message = "Send message")
        {
            return Created(string.Empty, SendToKafka(topic, message));
        }

        private object? SendToKafka(string topic, string message)
        {
            using (var producer = new ProducerBuilder<string, string>(_producerConfig).Build())
            {
                try
                {
                    return producer.ProduceAsync(topic,
                        new Message<string, string>
                        {
                            Key = DateTime.UtcNow.ToString(),
                            Value = message
                        })
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Oops, something went wrong: {ex.Message}");
                }
            }

            return null;
        }
    }
}