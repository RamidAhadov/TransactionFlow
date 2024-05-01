namespace TransactionFlow.Core.Constants;

public class InfoMessages
{
    public const string ItemAdded = "The given item was added to database.";
    public const string ItemUpdated = "The given item was updated in records.";
    public const string ItemDeleted = "The given item was removed from records.";
    public const string ZeroTransactionFound = "Customer has not any transaction history in given context.";
    public const string InsufficientFund = "You have not enough balance for this operation.";
    public const string InactiveAccount = "Customer's account is not an active account.";
    public const string KeyNotSet = "Key not set in database.";
}