using Confluent.Kafka;
using Newtonsoft.Json;
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

        //Serializing the object to Json string format.
        var producedMessage = JsonConvert.SerializeObject(message);

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            try
            {
                producer.Produce(_topic, new Message<Null, string> { Value = producedMessage });
                
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Failed to produce message: {e.Error.Reason}");
            }
        }
    }
}