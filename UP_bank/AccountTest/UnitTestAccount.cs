using AccountAPI.Controllers;
using AccountAPI.Services;
using AccountAPI.Settings;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using MongoDB.Driver;
namespace AccountTest
{
    public class UnitTestAccount
    {
        AccountsController _accountController;
        CreditCardsController _creditCardsController;
        TransactionsController _transactionsController;

        AccountService _accountService;
        CreditCardService _creditCardService;
        TransactionService _transactionService;

        public UnitTestAccount()
        {
            IMongoDataSettings settings = new MongoDataSettings();
            settings.AccountCollectionName = "Account";
            settings.AccountHistoryCollectionName = "AccountHistory";
            settings.DatabaseName = "Up_Bank_Test";
            settings.ConnectionString = "mongodb://root:Mongo%402024%23@localhost:27017";
            _accountService = new(settings);
            _creditCardService = new(settings);
            _transactionService = new(settings);

            _accountController = new(_accountService, _creditCardService);
            _creditCardsController = new(_creditCardService);
            _transactionsController = new(_transactionService, _accountService);
        }

        private static string _accNumber = "0276";
        private static string _accRestrictedNumber = "";
        private static string _agency = "123";

        private static readonly Address address = new Address
        {
            City = "Araraquara",
            Complement = "",
            Number = "100",
            State = "SP",
            Street = "Rua das Internets",
            ZipCode = "14802-020"
        };

        private static readonly List<Employee> employees = new List<Employee>
        {
            new Employee {
                Address = address,
                AddressNumber = address.Number,
                AddressZipCode = address.ZipCode,
                Cpf = "111.222.334-44",
                DtBirth = new DateTime(1990,10,10),
                Email = "email@teste.com",
                Income = 2000,
                Manager = true,
                Name = "Paulo José",
                Phone = "161111222",
                Register = 1,
                Sex = 'M'
            },
            new Employee {
                Address = address,
                AddressNumber = address.Number,
                AddressZipCode = address.ZipCode,
                Cpf = "555.666.777-88",
                DtBirth = new DateTime(1980,10,10),
                Email = "email2@teste.com",
                Income = 2000,
                Manager = false,
                Name = "Maria das Flores",
                Phone = "161111222",
                Register = 2,
                Sex = 'F'
            }
        };

        private static readonly List<AgencyEmployee> agencyEmployees = new List<AgencyEmployee>
        {
            new AgencyEmployee {
                AgencyNumber = "123",
                Cpf = "111.222.334-44"
            },
            new AgencyEmployee {
                AgencyNumber = "123",
                Cpf = "555.666.777-88"
            },
        };

        private static readonly List<Customer> customers = new List<Customer>
        {
            new Customer
            {
                Address = address,
                AddressNumber = address.Number,
                AddressZipCode = address.ZipCode,
                Cpf = "888.999.888-11",
                DtBirth = new DateTime(1992, 1, 1),
                Email = "cliente1@email.com",
                Income = 2000,
                Name = "Samuel Correia",
                Phone = "11111222",
                Restriction = false,
                Sex = 'M'
            },
            new Customer
            {
                Address = address,
                AddressNumber = address.Number,
                AddressZipCode = address.ZipCode,
                Cpf = "777.666.555-44",
                DtBirth = new DateTime(2010, 3, 10),
                Email = "cliente2@email.com",
                Income = 2000,
                Name = "Josefina Talta",
                Phone = "22222333",
                Restriction = false,
                Sex = 'F'
            }
        };

        private static readonly List<AgencyCustomerDTO> agencyCustomerDTOs = new List<AgencyCustomerDTO>
        {
            new AgencyCustomerDTO
            {
                Cpf = customers[0].Cpf,
                DtBirth = customers[0].DtBirth,
                Restriction = customers[0].Restriction
            },
            new AgencyCustomerDTO
            {
                Cpf = customers[1].Cpf,
                DtBirth = customers[1].DtBirth,
                Restriction = customers[1].Restriction
            }
        };

        private static readonly Agency agency = new Agency
        {
            Address = address,
            AddressNumber = address.Number,
            AddressZipCode = address.ZipCode,
            Cnpj = "123",
            Number = _agency,
            Restriction = false,
            Employees = employees,
            EmployeesCpf = agencyEmployees
        };

        private static readonly AccountAgencyDTO accountAgencyDTO = new AccountAgencyDTO
        {
            Number = agency.Number,
            Restriction = false
        };


        private static readonly CreditCard creditCard = new CreditCard
        {
            Active = false,
            CVV = "719",
            ExpirationDate = new DateTime(2029, 6, 1),
            Flag = "Visa",
            Limit = 5000,
            Name = "Samuel Correia",
            Number = 4761627983417534
        };

        private static readonly Account account = new Account
        {
            Agency = accountAgencyDTO,
            Balance = 0,
            Date = DateTime.Now,
            Extract = null,
            CreditCard = creditCard,
            Number = _accNumber,
            Restriction = false,
            Profile = EProfile.Normal,
            Overdraft = 1000,
            SavingsAccount = "0025-10",
            Customers = agencyCustomerDTOs
        };

        private static readonly AccountDTO adto = new AccountDTO
        {
            Agency = agency.Number,
            OwnerCpf = customers[0].Cpf,
            DependentCpf = customers[1].Cpf,
            Profile = "Normal"
        };

        public async Task<Account> CreateAccount(bool restricted, EProfile profile)
        {
            var result = await _accountService.CreateNewAccount(adto, accountAgencyDTO, customers, profile);
            result.Restriction = restricted;
            result.CreditCard = creditCard;
            await _accountService.Post(result);
            return result;
        }

        public async Task<Account> CreateAccount2(bool restricted, EProfile profile)
        {
            // Cria conta com agencia diferente para testar os metodos de excluir por agencia
            adto.Agency = "444";
            accountAgencyDTO.Number = "444";
            var result = await _accountService.CreateNewAccount(adto, accountAgencyDTO, customers, profile);
            result.Restriction = restricted;
            result.CreditCard = creditCard;
            await _accountService.Post(result);
            return result;
        }
        public async Task CreateTransaction(Account account, EType type, double price)
        {
            TransactionsDTO transactionsDTO = new TransactionsDTO
            {
                AccountDestinyNumber = "",
                Price = price,
                Type = type
            };
            await _transactionsController.CreateTransaction(account.Number, transactionsDTO);
        }

        [Fact]
        public async Task GetAccountDeleted()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            await _accountController.Delete(account.Number);

            var result = await _accountController.Get(account.Number, true);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAccountNotDeleted()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            var result = await _accountController.Get(account.Number, false);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllType0Normal()
        {
            var account = await CreateAccount(false, EProfile.Vip);
            account = await CreateAccount(false, EProfile.Normal);
            account = await CreateAccount(false, EProfile.University);
            var result = await _accountController.GetAll(0);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllType1Restricted()
        {
            var account = await CreateAccount(true, EProfile.Normal);
            account = await CreateAccount(false, EProfile.Vip);
            var result = await _accountController.GetAll(1);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllType2Loan()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            await CreateTransaction(account, EType.Loan, 1000);
            var result = await _accountController.GetAll(2);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetExtractId()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            await CreateTransaction(account, EType.Withdrawal, 50);
            var result = await _transactionsController.GetExtractId(account.Number, 1);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetExtractType()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            await CreateTransaction(account, EType.Deposit, 200);
            var result = await _transactionsController.GetExtractType(account.Number, 2);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetExtract()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            await CreateTransaction(account, EType.Deposit, 200);
            await CreateTransaction(account, EType.Withdrawal, 50);
            await CreateTransaction(account, EType.Loan, 500);
            var result = await _transactionsController.GetExtract(account.Number);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetCreditCard()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            var result = await _creditCardsController.Get(account.Number, 0);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task ActivateCreditCard()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            var result = await _creditCardsController.Active(account.Number);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllProfile()
        {
            EProfile profile = EProfile.Normal;
            var result = await _accountController.GetAllProfile(profile);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllByAgency()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            account = await CreateAccount(false, EProfile.Vip);
            var result = await _accountController.GetAllByAgency(account.Agency.Number);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateAccountRestriction()
        {
            var account = await CreateAccount(true, EProfile.Normal);
            AccountRestrictionDTO dto = new AccountRestrictionDTO { ManagerCpf = "123", Restriction = false };
            var result = await _accountController.UpdateAccountRestriction(account.Number, dto);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateAccountAgencyRestriction()
        {
            var account = await CreateAccount(true, EProfile.Normal);
            account = await CreateAccount(true, EProfile.Normal);
            AgencyRestrictionDTO dto = new AgencyRestrictionDTO { Restriction = false };
            var result = await _accountController.UpdateAccountAgencyRestriction(account.Agency.Number, dto);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateAccountCustomerRestriction()
        {
            var account = await CreateAccount(true, EProfile.Normal);
            account = await CreateAccount(true, EProfile.Normal);
            account.Customers[0].Restriction = true;
            CustomerRestrictionDTO dto = new CustomerRestrictionDTO { Restriction = false };
            var result = await _accountController.UpdateAccountCustomerRestriction(account.Customers[0].Cpf, dto);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateAccountProfile()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            EProfile profile = EProfile.Vip;
            var result = await _accountController.UpdateAccountProfile(account.Number, profile);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateAccountOverdraft()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            AccountOverdraftDTO dto = new AccountOverdraftDTO { Overdraft = 25000 };
            var result = await _accountController.UpdateAccountOverdraft(account.Number, dto);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Restore()
        {
            var account = await CreateAccount(true, EProfile.Normal);
            await _accountController.Delete(account.Number);
            var result = await _accountController.Restore(account.Number);
            Assert.IsType<OkObjectResult>(result.Result);
        }


        [Fact]
        public async Task DeleteByAgency()
        {
            var account = await CreateAccount2(false, EProfile.Normal);
            account = await CreateAccount2(false, EProfile.Normal);
            var result = await _accountController.DeleteByAgency(account.Agency.Number);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RestoreByAgency()
        {
            var account = await CreateAccount2(false, EProfile.Normal);
            account = await CreateAccount2(false, EProfile.Normal);
            await _accountController.DeleteByAgency(account.Agency.Number);
            var result = await _accountController.RestoreByAgency(account.Agency.Number);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task CheckBalance()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            var result = await _accountController.CheckBalance(account.Number);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllAccounts()
        {
            var account = await CreateAccount(false, EProfile.Normal);
            account = await CreateAccount(false, EProfile.Normal);
            await _accountController.Delete(account.Number);
            var result = await _accountController.GetAllAccounts();
            Assert.IsType<OkObjectResult>(result.Result);
        }
    }
}