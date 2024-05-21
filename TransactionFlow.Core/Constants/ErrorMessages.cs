using FluentResults;

namespace TransactionFlow.Core.Constants;

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
    public const string MaxAllowedAccountsExceed = "Cannot create new account. You reached maximum allowed account counts.";
    public const string CannotDeleteSingleAccount = "Cannot delete single account. Customer should has at least 2 accounts.";
    public const string CustomerHasNotAnotherAccount = "Cannot complete operation. Customer has not another active account for this process.";
    public const string AccountAlreadyDeactivated = "Cannot deactivate account. Because account is already inactive.";
    public const string AccountAlreadyActivated = "Cannot activate account. Because account is already active.";
    public const string ArchiveFailed = "An error occurred while archiving data.";
    public const string AccountsNotFound = "An error occurred while searching accounts.";
    public const string AccountNotDeleted = "Account could not be deleted.";
    public const string SenderIsReceiver = "Cannot complete operation. Sender and receiver accounts cannot be same.";
    public const string CannotGetTransactions = "An error occurred while get transactions.";
    public const string IdempotencyKeySearchError = "An error occurred while searching idempotency key in database.";
    public const string IdempotencyKeyNotSet = "An error occurred while add idempotency key to database.";
    public const string KeyNotGenerated = "New idempotency key not generated.";
    public const string WrongKeyFormat = "The format of the key is not a number format.";
    public const string TransferFailed = "Failed to transfer given amount.";
}