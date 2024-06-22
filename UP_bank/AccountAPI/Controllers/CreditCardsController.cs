using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditCardsController : ControllerBase
    {
        CreditCardService _creditCardService;
        public CreditCardsController(CreditCardService service)
        {
            _creditCardService = service;
        }

        [HttpGet("{accNumber},{cardNumber}")]
        // https://localhost:7244/api/creditcards/5725,1234567899990000 test
        public async Task<ActionResult<CreditCard>> Get(string accNumber, long cardNumber)
        {
            var account = await _creditCardService.GetAccount(accNumber);

            if (account == null)
                return NotFound("Account not found!");

            var creditCard = await _creditCardService.Get(account);
            if (creditCard == null)
                return NotFound("Credit card does not exist!");

            return Ok(creditCard);
        }


        [HttpPost("{accNumber}")]
        // https://localhost:7244/api/creditcards/5725 test
        public async Task<ActionResult<CreditCard>> Post(string accNumber, CreditCard creditCard)
        {
            var account = await _creditCardService.GetAccount(accNumber);

            if (account == null)
                return NotFound("Account not found!");

            if (account.Restriction == true)
                return BadRequest("Account is restricted!");

            return Ok(await _creditCardService.Post(accNumber, creditCard));
        }

    }
}
