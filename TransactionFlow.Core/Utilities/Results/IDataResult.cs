namespace TransactionFlow.Core.Utilities.Results;

public interface IDataResult<T>
{
    public bool Success { get; }
    public string Message { get; }
    public T Data { get; set; }
}