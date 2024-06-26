using AccountAPI.Settings;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Models;
using Models.DTO;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Transactions;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace AccountAPI.Services
{
    public class AccountService
    {
        private readonly IMongoCollection<Account> _accountCollection;
        private readonly IMongoCollection<Account> _accountHistoryCollection;
        private readonly HttpClient _httpClient = new HttpClient();

        public AccountService(IMongoDataSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _accountCollection = database.GetCollection<Account>(settings.AccountCollectionName);
            _accountHistoryCollection = database.GetCollection<Account>(settings.AccountHistoryCollectionName);
        }

        public async Task<Account> Get(string number, bool deleted)
        {
            Account account = null;
            account = (!deleted) ? await _accountCollection.Find(x => x.Number == number).FirstOrDefaultAsync() : await _accountHistoryCollection.Find(x => x.Number == number).FirstOrDefaultAsync();
            return account;
        }

        public async Task<List<Account>> GetAllProfile(EProfile profile, bool deleted)
        {
            List<Account> accounts = null;
            accounts = (!deleted) ? await _accountCollection.Find(x => x.Profile == profile).ToListAsync() : await _accountHistoryCollection.Find(x => x.Profile == profile).ToListAsync();
            return accounts;
        }

        public async Task<List<Account>> GetAllByAgency(string agency, bool deleted)
        {
            List<Account> accounts = null;
            accounts = (!deleted) ? await _accountCollection.Find(x => x.Agency.Number == agency).ToListAsync() : await _accountHistoryCollection.Find(x => x.Agency.Number == agency).ToListAsync();
            return accounts;
        }

        public async Task<List<Account>> GetAll(int param, bool deleted)
        {
            List<Account> accounts = null;

            if (param == 0) // GetAll
                accounts = (!deleted) ? await _accountCollection.Find(x => true).ToListAsync() : await _accountHistoryCollection.Find(x => true).ToListAsync();

            if (param == 1) // GetAll Restricted
                accounts = (!deleted) ? await _accountCollection.Find(x => x.Restriction == true).ToListAsync() : await _accountHistoryCollection.Find(x => x.Restriction == true).ToListAsync();

            if (param == 2) // GetAll Active Loan
            {
                List<Account> accountsLoan = new List<Account>();
                accounts = (!deleted) ? await _accountCollection.Find(x => true).ToListAsync() : await _accountHistoryCollection.Find(x => true).ToListAsync();
                foreach (var acc in accounts)
                {
                    if (acc.Extract != null)
                    {
                        if ((!accountsLoan.Exists(x => x.Number == acc.Number)) && (acc.Extract.Find(x => x.Type == EType.Loan) != null))
                        {
                            accountsLoan.Add(acc);
                        }
                    }
                }

                if (accountsLoan.Count == 0)
                    accountsLoan = null;

                accounts = accountsLoan;
            }

            return accounts;
        }

        public async Task<Account> GetHistory(string number)
        {
            return await _accountHistoryCollection.Find(x => x.Number == number).FirstOrDefaultAsync();
        }

        public async Task<Account> Post(Account account)
        {
            _accountCollection.InsertOne(account);
            return account;
        }

        public async Task UpdateAccountAgencyRestriction(string agencyNumber, AgencyRestrictionDTO agencyRestrictionDTO)
        {
            var accounts = await _accountCollection.Find(Builders<Account>.Filter.Empty).ToListAsync();
            foreach (var account in accounts)
            {
                if (account.Agency.Number == agencyNumber)
                    account.Agency.Restriction = agencyRestrictionDTO.Restriction;
            }

            foreach (var item in accounts)
            {
                var filter = Builders<Account>.Filter.Eq("Number", item.Number);
                var update = Builders<Account>.Update.Set("Agency.Restriction", item.Agency.Restriction);
                await _accountCollection.UpdateOneAsync(filter, update);
            }
        }

        public async Task UpdateAccountCustomerRestriction(string customerCPF, CustomerRestrictionDTO customerRestrictionDTO)
        {

            customerCPF = customerCPF.Replace(".", "").Replace("-", "").Trim();
            var accounts = await _accountCollection.Find(Builders<Account>.Filter.Empty).ToListAsync();
            var cpfMask = Convert.ToUInt64(customerCPF).ToString(@"000\.000\.000\-00");
            foreach (var account in accounts)
            {
                account.Customers.Find(c => c.Cpf == cpfMask).Restriction = customerRestrictionDTO.Restriction;
            }

            foreach (var item in accounts)
            {
                var filter = Builders<Account>.Filter.Eq("Number", item.Number);
                var update = Builders<Account>.Update.Set("Customers", item.Customers);
                await _accountCollection.UpdateOneAsync(filter, update);
            }

        }

        public async Task<Account> UpdateAccountRestriction(AccountRestrictionDTO dto, Account account)
        {
            var filter = Builders<Account>.Filter.Eq("Number", account.Number);
            var update = Builders<Account>.Update.Set("Restriction", dto.Restriction);
            await _accountCollection.UpdateOneAsync(filter, update);

            return await _accountCollection.Find(x => x.Number == account.Number).FirstOrDefaultAsync();
        }

        public async Task<Account> UpdateAccountProfile(EProfile profile, Account account)
        {
            var filter = Builders<Account>.Filter.Eq("Number", account.Number);
            var update = Builders<Account>.Update.Set("Profile", profile);
            await _accountCollection.UpdateOneAsync(filter, update);

            return await _accountCollection.Find(x => x.Number == account.Number).FirstOrDefaultAsync();
        }

        public async Task<Account> UpdateAccountOverdraft(AccountOverdraftDTO accountOverdraftDTO, Account account)
        {
            var filter = Builders<Account>.Filter.Eq("Number", account.Number);
            var update = Builders<Account>.Update.Set("Overdraft", accountOverdraftDTO.Overdraft);
            await _accountCollection.UpdateOneAsync(filter, update);

            return await _accountCollection.Find(x => x.Number == account.Number).FirstOrDefaultAsync();
        }

        public async Task<Account> UpdateAccountBalance(Transactions transaction, Account account)
        {
            int Type = (int)transaction.Type;
            double balance = 0;
            Account accountDestiny = null;

            if (transaction.Account != null)
                accountDestiny = await Get(transaction.Account.Number, false);

            // Update Account Origin Balance
            balance = (account.Balance + transaction.Price);

            var filter = Builders<Account>.Filter.Eq("Number", account.Number);
            var update = Builders<Account>.Update.Set("Balance", balance);
            await _accountCollection.UpdateOneAsync(filter, update);

            // Update Account Destiny Balance
            if (accountDestiny != null)
            {
                balance = accountDestiny.Balance + (transaction.Price * -1);
                var filterDestiny = Builders<Account>.Filter.Eq("Number", accountDestiny.Number);
                var updateDestiny = Builders<Account>.Update.Set("Balance", balance);
                await _accountCollection.UpdateOneAsync(filterDestiny, updateDestiny);
            }

            // Return Origin account info
            return await _accountCollection.Find(x => x.Number == account.Number).FirstOrDefaultAsync();
        }

        public async Task<Account> Delete(Account account)
        {
            try
            {
                _accountHistoryCollection.InsertOne(account);
                _accountCollection.DeleteOne(x => x.Number == account.Number);
                return account;
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error closing account: " + e.Message);
            }
        }

        public async Task<int> DeleteByAgency(List<Account> accounts)
        {
            int count = 0;
            try
            {
                foreach (var account in accounts)
                {
                    count++;
                    _accountHistoryCollection.InsertOne(account);
                    _accountCollection.DeleteOne(x => x.Number == account.Number);
                }
                return count;
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error closing account: " + e.Message);
            }
        }

        public async Task<Account> Restore(Account account)
        {
            _accountCollection.InsertOne(account);
            _accountHistoryCollection.DeleteOne(x => x.Number == account.Number);
            return account;
        }

        public async Task<int> RestoreByAgency(List<Account> accounts)
        {
            int count = 0;
            try
            {
                foreach (var account in accounts)
                {
                    count++;
                    _accountCollection.InsertOne(account);
                    _accountHistoryCollection.DeleteOne(x => x.Number == account.Number);
                }
                return count;
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error restoring account: " + e.Message);
            }
        }

        public async Task<List<Account>> BuildList()
        {
            var accounts = await GetAll(0, false);
            var accountHistory = await GetAll(0, true);
            var result = new List<Account>();
            foreach (var item in accounts)
            {
                result.Add(item);
            }
            foreach (var item in accountHistory)
            {
                result.Add(item);
            }
            return result;
        }

        public async Task<AccountAgencyDTO> GetAgency(AccountDTO accountDTO)
        {
            //GET API AGENCY
            AccountAgencyDTO agencyDTO = null;
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7142/api/Agencies/{accountDTO.Agency}");
                if (response.IsSuccessStatusCode)
                {
                    var agency = JsonConvert.DeserializeObject<Agency>(response.Content.ReadAsStringAsync().Result);
                    agencyDTO = new AccountAgencyDTO
                    {
                        Number = agency.Number,
                        Restriction = agency.Restriction
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

            return agencyDTO;
        }

        public async Task<List<Customer>> GetCustomer(AccountDTO accountDTO)
        {
            // GET API CUSTOMERS
            List<Customer>? listCustomers = new List<Customer>();
            Customer? customer = null;
            accountDTO.OwnerCpf = accountDTO.OwnerCpf.Replace(".", "").Replace("-", "").Trim();
            accountDTO.DependentCpf = accountDTO.DependentCpf.Replace(".", "").Replace("-", "").Trim();
            try
            {
                // Get customer owner
                if (accountDTO.OwnerCpf != String.Empty && accountDTO.OwnerCpf != null)
                {
                    var response = await _httpClient.GetAsync($"https://localhost:7147/api/Customers/{accountDTO.OwnerCpf}");
                    if (response.IsSuccessStatusCode)
                    {
                        customer = JsonConvert.DeserializeObject<Customer>(response.Content.ReadAsStringAsync().Result);
                        if (customer != null)
                        {
                            listCustomers.Add(customer);
                        }
                    }
                    // Ger customer dependent
                    if (accountDTO.DependentCpf != String.Empty && accountDTO.DependentCpf != null)
                    {
                        customer = null;
                        response = await _httpClient.GetAsync($"https://localhost:7147/api/Customers/{accountDTO.DependentCpf}");
                        if (response.IsSuccessStatusCode)
                        {
                            customer = JsonConvert.DeserializeObject<Customer>(response.Content.ReadAsStringAsync().Result);
                            if (customer != null)
                            {
                                listCustomers.Add(customer);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            if (listCustomers.Count == 0)
                listCustomers = null;

            return listCustomers;
        }

        public async Task<Account> CreateNewAccount(AccountDTO accountDTO, AccountAgencyDTO agency, List<Customer> customers, EProfile profile)
        {
            var allAccounts = await BuildList();
            var currentNumbers = allAccounts.Select(x => x.Number).ToList();

            #region Validate if account number already exists
            string accountNumber = String.Empty;
            do
            {
                accountNumber = new Random().Next(0, 1000).ToString().PadLeft(4, '0');
            } while (currentNumbers.Contains(accountNumber));
            #endregion

            List<AgencyCustomerDTO>? listCustomers = new List<AgencyCustomerDTO>();
            foreach (var customer in customers)
            {
                listCustomers.Add(new AgencyCustomerDTO
                {
                    Cpf = customer.Cpf,
                    DtBirth = customer.DtBirth,
                    Restriction = customer.Restriction
                });
            };

            // GET Overdraft
            double overdraft = 0;
            switch (profile)
            {
                case EProfile.University:
                    overdraft = 500;
                    break;
                case EProfile.Normal:
                    overdraft = 1000;
                    break;
                case EProfile.Vip:
                    overdraft = 3000;
                    break;
            }

            // Cria Account
            Account account = new Account
            {
                Agency = agency,
                Number = accountNumber,
                Date = DateTime.Today,  // Default
                Profile = profile,
                Customers = listCustomers,
                Overdraft = overdraft,
                Balance = 0,            // Default 
                Restriction = true,     // Restricted until manager approves
                CreditCard = null,      // Default
                Extract = null          // Default
            };
            return account;
        }

    }
}
