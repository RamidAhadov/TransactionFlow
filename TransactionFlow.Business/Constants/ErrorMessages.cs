using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.Business.Constants;

public static class ErrorMessages
{
    public const string IndexOutOfTheRange = "The entered index was out of the range. Please use a positive index.";
    public const string ObjectNotFound = "Item was not found in current context of search key was wrong.";
    public const string OperationFailed = "An error occurred while initiating the process.";
    public const string NullObjectEntered = "The entered object was null. Please make sure that entered object has an instance.";
    public const string AccountNotCreated = "Cannot create a new customer account. Please try again later.";
    public const string InvalidOperation = "Operation failed due to invalid input(s). Please make sure that inputs are int the correct format.";
    public const string IncorrectFormat = "Wrong format was entered. Please use an valid format.";
    public const string AccountNotFound = "Account could not be found. Plase make sure that account is exists.";
    public const string TransactionNotCreated = "An error occurred while create transaction.";
    public const string AdjustmentFailed = "An error occured while adjusting customer's balance.";
    public const string TransactionStatusError = "An error occured while change transaction status.";
}