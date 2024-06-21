using AccountAPI.Settings;
using Models;
using MongoDB.Driver;
using System.Transactions;

namespace AccountAPI.Services
{
    public class AccountService
    {
        private readonly IMongoCollection<Account> _accountCollection;

        public AccountService(IMongoDataSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _accountCollection = database.GetCollection<Account>(settings.AccountCollectionName);
        }

        public async Task<Account> Get(string number)
        {
            //var x = _accountCollection
            return await _accountCollection.Find(x => x.Number == number).FirstOrDefaultAsync();

        }

        public async Task<Account> CreateAccount(Account account)
        {
            _accountCollection.InsertOne(account);
            return account;
        }
        public async Task<Account> CreateTransaction(string number)
        {
            int Id = 0;
            var account = await Get(number);
            if (account == null)
                return null;

            List<Transactions> transactions = new List<Transactions>();

            if (account.Extract != null)
            {
                foreach (var item in account.Extract)
                {
                    transactions.Add(item);
                    Id = item.Id;
                }
            }

            Transactions transaction = new Transactions
            {
                Id = Id + 1,
                Price = 200,
                Type = EType.Deposit,
                Destiny = null,
                Date = DateTime.Now,
            };


            transactions.Add(transaction);


            var filter = Builders<Account>.Filter.Eq("Number", number);
            var update = Builders<Account>.Update.Set("Extract", transactions);
            await _accountCollection.UpdateOneAsync(filter, update);
            return account;
        }
    }
}
