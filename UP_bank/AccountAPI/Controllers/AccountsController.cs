using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using System.Net.Http;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        AccountService _accountService;
        CreditCardService _creditCardService;
        public AccountsController(AccountService service, CreditCardService creditCardService)
        {
            _accountService = service;
            _creditCardService = creditCardService;
        }

        [HttpGet("account/{accNumber}")]
        public async Task<ActionResult<Account>> Get(string accNumber, bool deleted = false)
        {
            var account = await _accountService.Get(accNumber, deleted);

            if (account == null) return NotFound("Account not found!");

            return Ok(account);
        }

        [HttpGet("type/{type}")]
        public async Task<ActionResult<Account>> GetAll(int type, bool deleted = false)
        {
            var accounts = await _accountService.GetAll(type, deleted);

            if (accounts == null) return NotFound("No accounts were found!");

            return Ok(accounts);
        }


        [HttpGet("profile/{profile}")]
        public async Task<ActionResult<Account>> GetAllProfile(EProfile profile, bool deleted = false)
        {
            var accounts = await _accountService.GetAllProfile(profile, deleted);

            if (accounts == null) return NotFound("No accounts were found!");

            return Ok(accounts);
        }

        [HttpGet("agency/{agency}")]
        public async Task<ActionResult<Account>> GetAllByAgency(string agency, bool deleted = false)
        {
            var accounts = await _accountService.GetAllByAgency(agency, deleted);

            if (accounts == null) return NotFound("No accounts were found!");

            return Ok(accounts);
        }

        [HttpPost]
        //https://localhost:7244/api/accounts/ test POST
        // aqui deve receber Account json
        public async Task<ActionResult<Account>> Post()
        {
            var account = await CreateNewAccount(); // temporario
            try
            {
                await _accountService.Post(await CreateNewAccount()); // temporario
                await _accountService.Post(account);
            }
            catch (Exception)
            {
                return BadRequest("Error creating account!");
            }

            var creditCard = await _creditCardService.Post(account); // create account credit card
            if (creditCard == null) return BadRequest("Account was created but without a credit card.");

            return Ok(account);
        }

        [HttpPatch("account/{accNumber}")]
        public async Task<ActionResult<Account>> UpdateAccountRestriction(string accNumber, AccountRestrictionDTO dto)
        {
            Account account = await _accountService.Get(accNumber, false);

            //dto.ManagerCpf = busca API funcionaro para ver se o CPF é manager
            //if true continua..
            if (account == null) return NotFound("Account not found");

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

        [HttpPatch("account/{accNumber}/profile/{profile}")]
        public async Task<ActionResult<Account>> UpdateAccountProfile(string accNumber, EProfile profile)
        {
            Account account = await _accountService.Get(accNumber, false);

            if (account == null) return NotFound("Account not found");

            if ((int)profile > 2) return NotFound("Profile informed does not exist!");

            if (account.Profile == profile) return BadRequest($"Account profile is already {account.Profile}");

            account = await _accountService.UpdateAccountProfile(profile, account);
            return Ok(account);
        }

        [HttpPatch("account/{accNumber}/overdraft/")]
        public async Task<ActionResult<Account>> UpdateAccountOverdraft(string accNumber, AccountOverdraftDTO accountOverdraftDTO)
        {
            Account account = await _accountService.Get(accNumber, false);

            if (account == null) return NotFound("Account not found");

            if (accountOverdraftDTO.Overdraft < 0) return BadRequest("Inform a positive value.");

            if (account.Overdraft == accountOverdraftDTO.Overdraft) return BadRequest($"Account overdraft is already $ {account.Overdraft}");

            account = await _accountService.UpdateAccountOverdraft(accountOverdraftDTO, account);
            return Ok(account);
        }

        [HttpDelete("close-account/account/{accNumber}")]
        public async Task<ActionResult> Delete(string accNumber)
        {
            Account account = await _accountService.Get(accNumber, false);
            if (account == null) return NotFound("Account not found");

            await _accountService.Delete(account);
            return Ok("Account successfully closed!");
        }

        [HttpDelete("close-account/agency/{agency}")]
        public async Task<ActionResult> DeleteByAgency(string agency)
        {
            var accounts = await _accountService.GetAllByAgency(agency, false);
            if (accounts == null) return NotFound("No accounts were found!");

            var result = await _accountService.DeleteByAgency(accounts);
            if (result == 0)
                return BadRequest("No accounts were found with this agency to delete.");

            return Ok("Accounts were successfully closed!");
        }

        [HttpPost("restore/{accNumber}")]
        public async Task<ActionResult<Account>> Restore(string accNumber)
        {
            Account account = await _accountService.Get(accNumber, true);

            if (account == null) return NotFound("Account not found");

            await _accountService.Restore(account);
            return Ok(account);
        }

        [HttpPost("restore/agency/{agency}")]
        public async Task<ActionResult<Account>> RestoreByAgency(string agency)
        {
            var accounts = await _accountService.GetAllByAgency(agency, true);

            if (accounts == null) return NotFound("No accounts were found!");

            var result = await _accountService.RestoreByAgency(accounts);
            if (result == 0)
                return BadRequest("No accounts were found with this agency to restore.");

            return Ok("Accounts were successfully restored!");
        }

        [HttpGet("checkBalance/account/{accNumber}")]
        public async Task<ActionResult<Account>> CheckBalance(string accNumber)
        {
            Account account = await _accountService.Get(accNumber, false);

            if (account == null) return NotFound("Account not found");

            AccountBalanceDTO accountBalanceDTO = new AccountBalanceDTO(account);
            if (accountBalanceDTO == null) return BadRequest("Error!");

            return Ok(accountBalanceDTO);
        }

        [HttpGet("getAllAccounts")]
        public async Task<ActionResult<List<Account>>> GetAllAccounts()
        {
            var accounts = await _accountService.GetAll(0, false);
            var acc_deleted = await _accountService.GetAll(0, true);
            var lst = _accountService.BuildList(accounts, acc_deleted);

            if (lst == null) return NotFound("No accounts were found!");

            return Ok(lst);
        }


        public async Task<Account> CreateNewAccount()
        {
            List<AgencyCustomerDTO> customerList = new List<AgencyCustomerDTO>();

            var accounts = await _accountService.GetAll(0, false);
            var acc_deleted = await _accountService.GetAll(0, true);
            var lst = _accountService.BuildList(accounts, acc_deleted);

            #region Validate if account number already exists
            string accountNumber = new Random().Next(0, 1000).ToString().PadLeft(4, '0');
            List<string> numbers = new List<string>();

            foreach (var item in accounts)
            {
                numbers.Add(item.Number);
            }

            do
            {
                accountNumber = new Random().Next(0, 1000).ToString().PadLeft(4, '0');
            } while (numbers.Contains(accountNumber));
            #endregion


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
                Number = accountNumber,
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