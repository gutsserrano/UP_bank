using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        TransactionService _transactionService;
        AccountService _accountService;
        public TransactionsController(TransactionService service, AccountService accountService)
        {
            _transactionService = service;
            _accountService = accountService;
        }

        [HttpGet("account/{accNumber}/id/{id}")]
        public async Task<ActionResult<Transactions>> GetExtractId(string accNumber, int id)
        {
            var account = await _transactionService.GetAccount(accNumber);
            if (account == null) return NotFound("Account not found!");

            Transactions transactions = null;
            transactions = await _transactionService.GetExtractId(account, id);

            if (transactions == null) return NotFound("Account does not have an extract with this id!");

            return Ok(transactions);
        }

        [HttpGet("account/{accNumber}/type/{type}")]
        public async Task<ActionResult<Transactions>> GetExtractType(string accNumber, int type)
        {
            if (type > 4) return BadRequest("This type does not exist.");

            var account = await _transactionService.GetAccount(accNumber);
            if (account == null) return NotFound("Account not found!");

            Transactions transactions = null;
            transactions = await _transactionService.GetExtractType(account, type);

            if (transactions == null) return NotFound("Account does not have an extract with this type!");

            return Ok(transactions);
        }

        [HttpGet("account/{accNumber}")]
        public async Task<ActionResult<List<Transactions>>> GetExtract(string accNumber)
        {
            List<Transactions> transactions = null;
            transactions = await _transactionService.GetExtract(accNumber);
            if (transactions == null) return NotFound("No extract found or account does not exists!");

            return Ok(transactions);
        }

        [HttpPost("account/{accNumber}")]
        public async Task<ActionResult<Transactions>> CreateTransaction(string accNumber, TransactionsDTO dto)
        {
            var account = await _transactionService.GetAccount(accNumber);
            if (account == null) return NotFound("Account not found!");

            var validation = await _transactionService.ValidateTransaction(account, dto);
            if (validation != "Ok") return BadRequest(validation);

            var transaction = await _transactionService.CreateTransaction(account, dto);

            await _accountService.UpdateAccountBalance(transaction, account);

            return Ok(transaction);
        }
    }
}
