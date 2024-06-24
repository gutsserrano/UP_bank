using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        TransactionService _transactionService;
        public TransactionsController(TransactionService service)
        {
            _transactionService = service;
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
        public async Task<ActionResult<Transactions>> Post(string accNumber, Transactions transaction)
        {
            var account = await _transactionService.GetAccount(accNumber);
            if (account == null)
                return NotFound("Account not found!");

            await _transactionService.Post(account, transaction);

            return Ok(transaction);
        }
    }
}
