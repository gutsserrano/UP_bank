using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
