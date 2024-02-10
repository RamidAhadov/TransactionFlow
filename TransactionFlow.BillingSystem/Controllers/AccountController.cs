using Microsoft.AspNetCore.Mvc;
using TransactionFlow.BillingSystem.Services.Abstraction;

namespace TransactionFlow.BillingSystem.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AccountController:ControllerBase
{
    private ITransferService _transferService;
    private IAccountService _accountService;

    public AccountController(ITransferService transferService, IAccountService accountService)
    {
        _transferService = transferService;
        _accountService = accountService;
    }

    /*[Route(nameof(TransferMoney))]
    [HttpPost]
    public async Task<IActionResult> TransferMoney(int sender, int receiver, decimal amount, decimal fee)
    {
        //1. Sender check
        var senderCheckTask = _accountManager.CheckSender(sender, amount, fee);

        //2. Receiver check
        var receiverCheckTask = _accountManager.CheckReceiver(receiver);

        await Task.WhenAll(senderCheckTask, receiverCheckTask);

        if (!senderCheckTask.Result.Success)
        {
            return BadRequest(senderCheckTask.Result.Message);
        }

        if (!receiverCheckTask.Result.Success)
        {
            return BadRequest(receiverCheckTask.Result.Message);
        }
        //3. Create transaction
        var transactionResult = await _transactionManager.CreateTransaction(sender, receiver, amount, fee);
        if (!transactionResult.Success)
        {
            return BadRequest(transactionResult.Message);
        }
        //4. Sender Negative adjust
        var senderResult = await _accountManager.TryNegativeAdjust(transactionResult.Data, senderCheckTask.Result.Data);
        if (!senderResult.Success)
        {
            return BadRequest(senderResult.Message);
        }
        //5. Receiver positive adjust
        var receiverResult =
            await _accountManager.TryPositiveAdjust(transactionResult.Data, receiverCheckTask.Result.Data);
        if (!receiverResult.Success)
        {
            //If positive adjustment fails, sender's balance rolls back.
            var rollbackCustomerBalance =
                await _accountManager.TryPositiveAdjust(transactionResult.Data, senderCheckTask.Result.Data);
            if (!rollbackCustomerBalance.Success)
            {
                return BadRequest("Balance rollback failed.");
            }
            return BadRequest(receiverResult.Message);
        }

        //6. Switch transaction status to true.\
        var statusChangeResult = await _transactionManager.ChangeTransactionStatus(transactionResult.Data);
        if (!statusChangeResult.Success)
        {
            return BadRequest();
        }
        
        return Ok();
    }
*/

    [Route(nameof(TransferMoneyAsync))]
    [HttpPost]
    public async Task<IActionResult> TransferMoneyAsync(int sender, int receiver, decimal amount, decimal fee)
    {
        var result = await _transferService.TransferMoneyAsync(sender, receiver, amount, fee);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }

    [Route(nameof(CreateAccountAsync))]
    [HttpPost]
    public async Task<IActionResult> CreateAccountAsync(int customerId)
    {
        var result = await _accountService.CreateAccountAsync(customerId);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }
}