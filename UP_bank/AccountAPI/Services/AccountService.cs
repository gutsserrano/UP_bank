using AccountAPI.Settings;
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

        public async Task<Account> Get(string number, int deleted)
        {
            Account account = null;
            account = (deleted == 0) ? await _accountCollection.Find(x => x.Number == number).FirstOrDefaultAsync() : await _accountHistoryCollection.Find(x => x.Number == number).FirstOrDefaultAsync();
            return account;
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
            var filter = Builders<Account>.Filter.Eq("Number", dto.Number);
            var update = Builders<Account>.Update.Set("Restriction", dto.Restriction);
            await _accountCollection.UpdateOneAsync(filter, update);

            return await _accountCollection.Find(x => x.Number == dto.Number).FirstOrDefaultAsync();
        }

        public async Task<Account> UpdateAccountBalance(Transactions transaction, Account account)
        {
            int Type = (int)transaction.Type;
            double balance = 0;
            Account accountDestiny = null;

            if (transaction.Destiny != null)
                accountDestiny = await Get(transaction.Destiny.Number, 0);

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
            _accountHistoryCollection.InsertOne(account);
            _accountCollection.DeleteOne(x => x.Number == account.Number);
            return account;
        }

        public async Task<Account> Restore(Account account)
        {
            _accountCollection.InsertOne(account);
            _accountHistoryCollection.DeleteOne(x => x.Number == account.Number);
            return account;
        }
    }
}
