using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

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

        [HttpGet("{number}")]
        // https://localhost:7244/api/accounts/5725 test
        public async Task<ActionResult<Account>> Get(string number)
        {
            return await _accountService.Get(number);
        }

        [HttpPost("1")]
        //https://localhost:7244/api/accounts/1 test
        public async Task<ActionResult<Account>> CreateAccount()
        {
            var account = CriaConta();
            await _accountService.CreateAccount(account);
            return Ok(account);
        }

        
        [HttpPost("2/{number}")]
        //https://localhost:7244/api/accounts/2/5725 test
        public async Task<ActionResult<Account>> CreateTransaction(string number)
        {
            var account = await _accountService.CreateTransaction(number);
            return Ok(account);
        }



        public Account CriaConta()
        {
            Address address = new Address
            {
                City = "Araraquara",
                Number = "100",
                State = "SP",
                Complement = "",
                Street = "Rua 7 de Setembro",
                ZipCode = "14802-020"
            };

            Employee employee = new Employee
            {
                Name = "Joao da Silva",
                Phone = "163333-1111",
                Sex = 'M',
                Register = 10,
                Address = address,
                AddressNumber = "10",
                AddressZipCode = "14802-020",
                Cpf = "111.222.333-45",
                DtBirth = new DateTime(1990, 10, 5),
                Email = "funcionario@google.com",
                Income = 2000,
                Manager = false
            };

            Employee employee2 = new Employee
            {
                Name = "Maria das Neves",
                Phone = "163333-2222",
                Sex = 'F',
                Register = 5,
                Address = address,
                AddressNumber = "15",
                AddressZipCode = "14802-020",
                Cpf = "555.666.777-89",
                DtBirth = new DateTime(1970, 2, 5),
                Email = "gerente@google.com",
                Income = 4000,
                Manager = true
            };

            Agency agency = new Agency
            {
                Cnpj = "12.345.678/0001-10",
                Number = "0064",
                Restriction = false,
                Employees = new List<Employee> { employee, employee2 },
                Address = address
            };

            Customer customer = new Customer
            {
                Address = address,
                AddressNumber = "200",
                AddressZipCode = "14802-020",
                Cpf = "444.777.222-00",
                Email = "cliente1@google.com",
                Income = 5000,
                Name = "Pedro Henrique Silva",
                Phone = "193331-1222",
                Restriction = false,
                Sex = 'M',
                DtBirth = new DateTime(1992, 10, 10)
            };

            Customer customer2 = new Customer
            {
                Address = address,
                AddressNumber = "200",
                AddressZipCode = "14802-020",
                Cpf = "444.777.222-00",
                Email = "cliente2@google.com",
                Income = 5000,
                Name = "Joaozinho Silva",
                Phone = "193331-1222",
                Restriction = false,
                Sex = 'M',
                DtBirth = new DateTime(2015, 5, 5)
            };

            CreditCard creditCard = new CreditCard
            {
                CVV = "007",
                ExpirationDate = new DateTime(2030, 12, 31),
                Flag = "Visa",
                Limit = 10000,
                Name = "Pedro Silva",
                Number = 1234567899990000
            };

            Account account = new Account
            {
                Agency = agency,
                Balance = 500,
                CreditCard = creditCard,
                Number = "5725",
                Date = DateTime.Today,
                Overdraft = 1000,
                Profile = EProfile.Normal,
                Restriction = false,
                Customers = new List<Customer> { customer, customer2 },
                Extract = null
            };

            return account;

        }

    }
}
