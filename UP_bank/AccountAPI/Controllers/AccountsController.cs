using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        AccountService _accountService;
        public AccountsController(AccountService service)
        {
            _accountService = service;
        }

        [HttpGet("account/{accNumber}/status/{deleted}")]
        public async Task<ActionResult<Account>> Get(string accNumber, int deleted)
        {
            var account = await _accountService.Get(accNumber, deleted);

            if (account == null) return NotFound("Account not found!");

            return Ok(account);
        }

        [HttpGet("status/{deleted}/param/{param}")]
        public async Task<ActionResult<Account>> GetAll(int param, int deleted)
        {
            var account = await _accountService.GetAll(deleted, param);

            if (account == null) return NotFound("No accounts were located!");

            return Ok(account);
        }

        [HttpGet("status/{deleted}/profile/{profile}")]
        public async Task<ActionResult<Account>> GetAllProfile(EProfile profile, int deleted)
        {
            var account = await _accountService.GetAllProfile(profile, deleted);

            if (account == null) return NotFound("No accounts were located!");

            return Ok(account);
        }

        [HttpPost]
        //https://localhost:7244/api/accounts/ test POST
        // aqui deve receber Account json
        public async Task<ActionResult<Account>> Post()
        {
            var account = CriaContaTemp(); // temporario
            try
            {
                await _accountService.Post(CriaContaTemp()); // temporario
                await _accountService.Post(account);
            }
            catch (Exception)
            {
                return BadRequest("Error creating account!");
            }
            return Ok(account);
        }

        [HttpPatch("account/{accNumber}")]
        public async Task<ActionResult<Account>> UpdateAccountRestriction(string accNumber, AccountRestrictionDTO dto)
        {
            Account account = await _accountService.Get(accNumber, 0);

            //dto.ManagerCpf = busca API funcionaro para ver se o CPF é manager
            //if true continua..
            if (account == null) return NotFound();

            if (dto.Restriction == account.Restriction) return BadRequest($"Account is already in restriction status {account.Restriction}");

            account = await _accountService.UpdateAccountRestriction(dto, account);
            return Ok(account);
        }

        [HttpPatch("agency/{agencyNumber}")]
        public async Task<ActionResult<Account>> UpdateAccountAgencyRestriction(string agencyNumber, AgencyRestrictionDTO agencyRestrictionDTO)
        {
            await _accountService.UpdateAccountAgencyRestriction(agencyNumber, agencyRestrictionDTO);

            return Ok("Agency restriction updated.");
        }

        [HttpPatch("customer/{customerCPF}")]
        public async Task<ActionResult<Account>> UpdateAccountCustomerRestriction(string customerCPF, CustomerRestrictionDTO customerRestrictionDTO)
        {
            await _accountService.UpdateAccountCustomerRestriction(customerCPF, customerRestrictionDTO);

            return Ok("Customer restriction updated.");
        }

        [HttpDelete("account/{accNumber}")]
        public async Task<ActionResult> Delete(string accNumber)
        {
            Account account = await _accountService.Get(accNumber, 0);

            if (account == null) return NotFound("Account not found!");

            await _accountService.Delete(account);
            return Ok("Account successfully deleted!");
        }

        [HttpPost("restore/{accNumber}")]
        public async Task<ActionResult<Account>> Restore(string accNumber)
        {
            Account account = await _accountService.Get(accNumber, 1);

            if (account == null) return NotFound("Account not found!");

            await _accountService.Restore(account);
            return Ok(account);
        }
        [HttpGet("checkBalance/account/{accNumber}")]
        public async Task<ActionResult<Account>> CheckBalance(string accNumber)
        {
            Account account = await _accountService.Get(accNumber, 0);

            if (account == null) return NotFound("Account not found!");

            AccountBalanceDTO accountBalanceDTO = new AccountBalanceDTO(account);
            if(accountBalanceDTO == null) return BadRequest("Error!");

            return Ok(accountBalanceDTO);
        }


        public Account CriaContaTemp()
        {
            List<AgencyCustomerDTO> customerList = new List<AgencyCustomerDTO>();

            customerList.Add(new AgencyCustomerDTO
            {
                Cpf = "555.666.888-99",
                DtBirth = new DateTime(1990, 10, 5),
                Restriction = false
            });

            customerList.Add(new AgencyCustomerDTO
            {
                Cpf = "444.777.222-00",
                DtBirth = new DateTime(2014, 2, 10),
                Restriction = false
            });

            AccountAgencyDTO agency = new AccountAgencyDTO
            {
                Number = "0064",
                Restriction = false,
            };

            // Cria Account
            Account account = new Account
            {
                Agency = agency,
                Number = new Random().Next(0, 1000).ToString().PadLeft(4, '0'),
                Date = DateTime.Today,
                Profile = EProfile.Normal,
                Customers = customerList,
                Overdraft = 1000,
                Balance = 0,
                Restriction = true, // restrito true até ser aceito pelo gerente
                CreditCard = null, // nulo até gerar o cartao
                Extract = null // nulo pois nao tem transacoes ainda
            };
            return account;
        }
    }
}