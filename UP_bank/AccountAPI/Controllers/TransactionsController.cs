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

            var validation = await ValidateTransaction(accNumber, dto);
            if (validation != "Ok") return BadRequest(validation);

            var transaction = await _transactionService.CreateTransaction(account, dto);

            await _accountService.UpdateAccountBalance(transaction, account);

            return Ok(transaction);
        }

        public async Task<string> ValidateTransaction(string accNumber, TransactionsDTO dto)
        {
            int Type = (int)dto.Type;
            var account = await _transactionService.GetAccount(accNumber);
            var accountDestiny = await _transactionService.GetAccount(dto.AccountDestinyNumber);

            if (Type != 3 && accountDestiny != null) return "Operation not allowed, you can only inform the Destiny Account in a Transfer type transaction!";

            if (Type == 0 || Type == 3 || Type == 4) // Subtract balance
            {
                if (accountDestiny == null && Type == 3) return "Destiny account not located!";
                if (dto.Price > (account.Balance + account.Overdraft)) return $"Your Account Balance is $ {account.Balance} and your Overdraft is $ {account.Overdraft}, with Total of $ {account.Balance + account.Overdraft} available. This value is lower than Transaction value of $ {dto.Price}!";
            }

            if (accountDestiny == null)
            {
                if (account.Agency.Restriction == true) return "Account Agency is restricted!";
                if (account.Restriction == true) return "Account Origin is restricted!";
                if (account.Customers[0].Restriction == true) return "Account Origin Customer is restricted!";
            }
            else
            {
                if (accountDestiny.Agency.Restriction == true) return "Account Destiny Agency is restricted!";
                if (accountDestiny.Restriction == true) return "Account Destiny is restricted!";
                if (accountDestiny.Customers[0].Restriction == true) return "Account Destiny Customer is restricted!";
                if (account.Number == accountDestiny.Number) return "You cannot transfer to your own account! Use Deposit instead.";
            }

            return "Ok";
        }
    }
}
