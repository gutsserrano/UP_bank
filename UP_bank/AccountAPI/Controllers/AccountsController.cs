using AccountAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

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

        [HttpGet("{number},{deleted}")]
        // https://localhost:7244/api/accounts/5725,0 test GET
        public async Task<ActionResult<Account>> Get(string number, int deleted)
        {
            var account = await _accountService.Get(number, deleted);

            if (account == null)
                return NotFound();

            return Ok(account);
        }

        [HttpPost]
        //https://localhost:7244/api/accounts/ test POST
        // aqui deve receber Account json
        public async Task<ActionResult<Account>> Post()
        {
            var account = CriaContaTemp(); // temporario
            await _accountService.Post(account);
            return Ok(account);
        }

        [HttpPatch]
        //https://localhost:7244/api/accounts/ test PATCH
        public async Task<ActionResult<Account>> Patch(AccountRestrictionDTO dto)
        {
            Account account = await _accountService.Get(dto.Number, 0);

            if (account == null)
                return NotFound();

            if (dto.Restriction == account.Restriction)
                return BadRequest($"Account is already in restriction status {account.Restriction}");

            account = await _accountService.UpdateAccountRestriction(dto, account);
            return Ok(account);
        }

        [HttpPut("5/{agencyNumber}")]
        public async Task<ActionResult<Account>> UpdateAccountAgencyRestriction(string agencyNumber, AgencyRestrictionDTO agencyRestrictionDTO)
        {
            await _accountService.UpdateAccountAgencyRestriction(agencyNumber, agencyRestrictionDTO);

            return Ok();
        }

        [HttpPut("6/{customerCPF}")]
        public async Task<ActionResult<Account>> UpdateAccountCustomerRestriction(string customerCPF, CustomerRestrictionDTO customerRestrictionDTO)
        {
            await _accountService.UpdateAccountCustomerRestriction(customerCPF, customerRestrictionDTO);

            return Ok();
        }

        [HttpDelete("{number}")]
        public async Task<ActionResult> Delete(string number)
        {
            Account account = await _accountService.Get(number, 0);

            if (account == null)
                return NotFound();

            await _accountService.Delete(account);
            return Ok("Account successfully deleted!");
        }

        [HttpPost("restore/{number}")]
        public async Task<ActionResult<Account>> Restore(string number)
        {
            Account account = await _accountService.Get(number, 1);

            if (account == null)
                return NotFound();

            await _accountService.Restore(account);
            return Ok(account);
        }


        public Account CriaContaTemp()
        {
            // Dados de Entrada
            // Address, lista de Customers, lista de Employees e Agencia
            Address address = new Address
            {
                City = "Araraquara",
                Number = "100",
                State = "SP",
                Complement = "",
                Street = "Rua 7 de Setembro",
                ZipCode = "14802-020"
            };

            List<Customer> customerList = new List<Customer>();
            List<Employee> employeeList = new List<Employee>();


            employeeList.Add(new Employee
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
            }
            );

            employeeList.Add(new Employee
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
            });


            customerList.Add(new Customer
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
            });


            customerList.Add(new Customer
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
            });


            // Dados da Agencia
            Agency agency = new Agency
            {
                Cnpj = "12.345.678/0001-10",
                Number = "0064",
                Restriction = false,
                Employees = employeeList,
                Address = address
            };

            // Cria Account
            Account account = new Account
            {
                Agency = agency,
                Number = "5726",
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

// JSON DE POST
/*
{
    "number": "5725",
    "agency": {
        "number": "0064",
        "address": {
            "zipCode": "14802-020",
            "number": "100",
            "street": "Rua 7 de Setembro",
            "complement": "",
            "city": "Araraquara",
            "state": "SP"
        },
        "cnpj": "12.345.678/0001-10",
        "employees": [
            {
                "manager": false,
                "register": 10,
                "cpf": "111.222.333-45",
                "name": "Joao da Silva",
                "dtBirth": "1990-10-05T00:00:00",
                "sex": "M",
                "income": 2000,
                "phone": "163333-1111",
                "email": "funcionario@google.com",
                "address": {
                    "zipCode": "14802-020",
                    "number": "100",
                    "street": "Rua 7 de Setembro",
                    "complement": "",
                    "city": "Araraquara",
                    "state": "SP"
                }
            },
            {
                "manager": true,
                "register": 5,
                "cpf": "555.666.777-89",
                "name": "Maria das Neves",
                "dtBirth": "1970-02-05T00:00:00",
                "sex": "F",
                "income": 4000,
                "phone": "163333-2222",
                "email": "gerente@google.com",
                "address": {
                    "zipCode": "14802-020",
                    "number": "100",
                    "street": "Rua 7 de Setembro",
                    "complement": "",
                    "city": "Araraquara",
                    "state": "SP"
                }
            }
        ],
        "restriction": false
    },
    "customers": [
        {
            "restriction": false,
            "cpf": "444.777.222-00",
            "name": "Pedro Henrique Silva",
            "dtBirth": "1992-10-10T00:00:00",
            "sex": "M",
            "income": 5000,
            "phone": "193331-1222",
            "email": "cliente1@google.com",
            "address": {
                "zipCode": "14802-020",
                "number": "100",
                "street": "Rua 7 de Setembro",
                "complement": "",
                "city": "Araraquara",
                "state": "SP"
            }
        },
        {
            "restriction": false,
            "cpf": "444.777.222-00",
            "name": "Joaozinho Silva",
            "dtBirth": "2015-05-05T00:00:00",
            "sex": "M",
            "income": 5000,
            "phone": "193331-1222",
            "email": "cliente2@google.com",
            "address": {
                "zipCode": "14802-020",
                "number": "100",
                "street": "Rua 7 de Setembro",
                "complement": "",
                "city": "Araraquara",
                "state": "SP"
            }
        }
    ],
    "restriction": false,
    "creditCard": null,
    "overdraft": 1000,
    "profile": 1,
    "date": "2024-06-22T00:00:00-03:00",
    "balance": 500,
    "extract": null
}
*/
