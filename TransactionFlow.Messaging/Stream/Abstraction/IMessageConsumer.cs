namespace TransactionFlow.Messaging.Stream.Abstraction;

public interface IMessageConsumer
{
    IEnumerable<string> Consume();
}