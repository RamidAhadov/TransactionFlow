namespace TransactionFlow.Core.Utilities.Results;

public interface IDataResult<T> where T: class
{
    public bool Success { get; }
    public string Message { get; }
    public T Data { get; set; }
}