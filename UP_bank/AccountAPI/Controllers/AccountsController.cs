using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using System;
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
        public async Task<ActionResult<Account>> Post(AccountDTO accountDTO)
        {
            try
            {
                EProfile profile = (EProfile)Enum.Parse(typeof(EProfile), accountDTO.Profile);

                var agency = await _accountService.GetAgency(accountDTO);       // Get Agency DTO Api
                if (agency == null) return NotFound("Agency not found!");

                var customers = await _accountService.GetCustomer(accountDTO);  // Get List Customers DTO Api
                if (customers == null) return NotFound("Customers not found!");

                var account = await _accountService.CreateNewAccount(accountDTO, agency, customers, profile);    // Create Account

                if (account == null) return BadRequest("Could not create an account at this time!");

                await _accountService.Post(account);    // Post Account

                var creditCard = await _creditCardService.Post(customers, account); // Post Credit Card to Account
                if (creditCard == null) return BadRequest("Account was created but without a credit card.");
                account.CreditCard = creditCard;

                return Ok(account);
            }
            catch (Exception)
            {
                return BadRequest("Error creating account!");
            }
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
            var lst = _accountService.BuildList();

            if (lst == null) return NotFound("No accounts were found!");

            return Ok(lst);
        }
    }
}