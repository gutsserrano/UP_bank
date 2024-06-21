using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
