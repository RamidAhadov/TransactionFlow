using Confluent.Kafka;
using TransactionFlow.Messaging.Stream.Abstraction;

namespace TransactionFlow.Messaging.Stream.Concrete;

public class KafkaMessageProducer:IMessageProducer
{
    private string _endPoint;
    private string _topic;
    
    public KafkaMessageProducer(string topic, string endPoint)
    {
        _topic = topic;
        _endPoint = endPoint;
    }
    public void ProduceMessage(object message)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _endPoint,
        };

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            try
            {
                var deliveryReport = producer.ProduceAsync(_topic, new Message<Null, string> { Value = (string)message }).Result;
                Console.WriteLine($"Produced message '{message}' to topic {deliveryReport.Topic}, partition {deliveryReport.Partition}, offset {deliveryReport.Offset}");
                
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Failed to produce message: {e.Error.Reason}");
            }
        }
    }
}