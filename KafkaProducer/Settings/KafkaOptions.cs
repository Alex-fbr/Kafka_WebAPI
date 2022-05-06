namespace KafkaProducer.Settings
{
    public class KafkaOptions
    {
        public string BootstrapServers { get; set; } // если локально "localhost:9092", еслив докере "kafka:9092"
    }
}
