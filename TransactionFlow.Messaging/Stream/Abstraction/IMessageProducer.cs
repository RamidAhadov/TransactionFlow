namespace TransactionFlow.Messaging.Stream.Abstraction;

public interface IMessageProducer
{
    void ProduceMessage(object message);
}