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

        [HttpGet("{accNumber},{id}")]
        public async Task<ActionResult<Transactions>> Get(string accNumber, int id)
        {
            var account = await _transactionService.GetAccount(accNumber);
            if (account == null)
                return NotFound("Account not found!");

            Transactions transactions = null;
            transactions = await _transactionService.Get(account, id);

            if (transactions == null)
                return NotFound("Account does not have an extract with this id!");

            return Ok(transactions);
        }

        [HttpGet("{accNumber}")]
        public async Task<ActionResult<List<Transactions>>> GetAll(string accNumber)
        {
            List<Transactions> transactions = null;
            transactions = await _transactionService.GetAll(accNumber);
            if (transactions == null)
                return NotFound();

            return Ok(transactions);
        }

        [HttpPost("{accNumber}")]
        public async Task<ActionResult<Transactions>> Post(string accNumber, TransactionsDTO dto)
        {
            var account = await _transactionService.GetAccount(accNumber);
            if (account == null) return NotFound("Account not found!");

            var validation = await ValidateTransaction(accNumber, dto);
            if (validation != "Ok") return BadRequest(validation);

            var transaction = await _transactionService.CreateTransactions(account, dto);

            await _accountService.UpdateBalances(transaction, account);

            return Ok(transaction);
        }

        public async Task<string> ValidateTransaction(string accNumber, TransactionsDTO dto)
        {
            int Type = (int)dto.Type;
            var account = await _transactionService.GetAccount(accNumber);
            var accountDestiny = await _transactionService.GetAccount(dto.AccountDestinyNumber);

            if (Type == 0 || Type == 3 || Type == 4) // Subtract balance
            {
                if (accountDestiny == null && Type == 3) return "Destiny account not located!";

                if (dto.Price > account.Balance) return "Account Balance is lower than Transaction value!";
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
            }

            return "Ok";
        }
    }
}
