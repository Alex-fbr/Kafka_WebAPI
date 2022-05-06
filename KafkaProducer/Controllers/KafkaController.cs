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
        private readonly string topic = "simpletalk_topic";
        private readonly ProducerConfig config;

        public KafkaController(ILogger<KafkaController> logger, IOptions<KafkaOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            config = new()
            {
                BootstrapServers = _options.BootstrapServers
            };
        }

        [HttpGet("/getMessages")]
        public IEnumerable<WeatherForecast> Read()
        {
            var Summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("/send")]
        public IActionResult Send([FromQuery] string message = "Send message")
        {
            return Created(string.Empty, SendToKafka(topic, message));
        }

        private object? SendToKafka(string topic, string message)
        {
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    return producer.ProduceAsync(topic, new Message<Null, string> { Value = message })
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Oops, something went wrong: {e}");
                    _logger.LogError(e, $"Oops, something went wrong: {e.Message}");
                }
            }

            return null;
        }
    }
}