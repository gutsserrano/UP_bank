using AccountAPI.Settings;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Models;
using Models.DTO;
using MongoDB.Driver;
using System.Transactions;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace AccountAPI.Services
{
    public class AccountService
    {
        private readonly IMongoCollection<Account> _accountCollection;
        private readonly IMongoCollection<Account> _accountHistoryCollection;

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
                    if(acc.Extract != null)
                    {
                        if ((!accountsLoan.Exists(x => x.Number == acc.Number)) && (acc.Extract.Find(x => x.Type == EType.Loan) != null))
                        {
                            accountsLoan.Add(acc);
                        }
                    }
                }

                if(accountsLoan.Count == 0)
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

        public async Task<Account> Restore(Account account)
        {
            _accountCollection.InsertOne(account);
            _accountHistoryCollection.DeleteOne(x => x.Number == account.Number);
            return account;
        }

        public  List<Account> buildList(List<Account> acc, List <Account> acc2)
        {
            var result = new List<Account>();
            foreach (var item in acc)
            {
                result.Add(item);
            }
            foreach (var item in acc2)
            {
                result.Add(item);
            }
            return result;
        }
        
    }
}
