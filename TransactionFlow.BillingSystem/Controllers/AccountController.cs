using Microsoft.AspNetCore.Mvc;
using TransactionFlow.Business.Abstraction;

namespace TransactionFlow.BillingSystem.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AccountController:ControllerBase
{
    private IAccountService _accountService;
    private ITransactionService _transactionService;

    public AccountController(IAccountService accountService, ITransactionService transactionService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
    }

    [Route(nameof(TransferMoney))]
    [HttpPost]
    public async Task<IActionResult> TransferMoney(int sender, int receiver, decimal amount, decimal fee)
    {
        //1. Sender check
        var senderCheckTask = _accountService.CheckSender(sender, amount, fee);

        //2. Receiver check
        var receiverCheckTask = _accountService.CheckReceiver(receiver);

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
        var transactionResult = await _transactionService.CreateTransaction(sender, receiver, amount, fee);
        if (!transactionResult.Success)
        {
            return BadRequest(transactionResult.Message);
        }
        //4. Sender Negative adjust
        var senderResult = await _accountService.TryNegativeAdjust(transactionResult.Data, senderCheckTask.Result.Data);
        if (!senderResult.Success)
        {
            return BadRequest(senderResult.Message);
        }
        //5. Receiver positive adjust
        var receiverResult =
            await _accountService.TryPositiveAdjust(transactionResult.Data, receiverCheckTask.Result.Data);
        if (!receiverResult.Success)
        {
            //If positive adjustment fails, sender's balance rolls back.
            var rollbackCustomerBalance =
                await _accountService.TryPositiveAdjust(transactionResult.Data, senderCheckTask.Result.Data);
            if (!rollbackCustomerBalance.Success)
            {
                return BadRequest("Balance rollback failed.");
            }
            return BadRequest(receiverResult.Message);
        }

        //6. Switch transaction status to true.\
        var statusChangeResult = await _transactionService.ChangeTransactionStatus(transactionResult.Data);
        if (!statusChangeResult.Success)
        {
            return BadRequest();
        }
        
        return Ok();
    }
}