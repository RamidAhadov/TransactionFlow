using Confluent.Kafka;
using TransactionFlow.Messaging.Stream.Abstraction;

namespace TransactionFlow.Messaging.Stream.Concrete;

public class KafkaMessageConsumer:IMessageConsumer
{
    private string _endPoint;
    private string _topic;
    
    public KafkaMessageConsumer(string topic, string endPoint)
    {
        _topic = topic;
        _endPoint = endPoint;
    }
    public IEnumerable<string> Consume()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _endPoint,
            GroupId = "my-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            consumer.Subscribe(_topic);

            while (true)
            {
                var consumeResult = consumer.Consume();
                yield return consumeResult.Message.Value; 
            }
        }
    }
}