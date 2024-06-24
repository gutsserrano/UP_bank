using AccountAPI.Settings;
using Models;
using Models.DTO;
using MongoDB.Driver;

namespace AccountAPI.Services
{
    public class TransactionService
    {
        private readonly IMongoCollection<Account> _accountCollection;

        public TransactionService(IMongoDataSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _accountCollection = database.GetCollection<Account>(settings.AccountCollectionName);
        }

        public async Task<Transactions> Get(Account account, int id)
        {
            List<Transactions>? transactions = null;
            Transactions? transaction = null;

            transactions = account.Extract;

            if (transactions != null)
                transaction = transactions.Find(x => x.Id == id);

            return transaction;
        }

        public async Task<Account> GetAccount(string accNumber)
        {
            var account = await _accountCollection.Find(x => x.Number == accNumber).FirstOrDefaultAsync();
            return account;
        }

        public async Task<List<Transactions>> GetAll(string number)
        {
            List<Transactions>? transactions = null;
            var account = await _accountCollection.Find(x => x.Number == number).FirstOrDefaultAsync();

            if (account != null)
                transactions = account.Extract;

            return transactions;
        }

        public async Task<Transactions> Post(Account account, TransactionsDTO dto)
        {
            int Id = 0;

            List<Transactions> transactions = new List<Transactions>();

            // Load all existing extracts
            if (account.Extract != null)
            {
                foreach (var item in account.Extract)
                {
                    transactions.Add(item);
                    Id = item.Id;
                }
            }

            // Set transaction values
            var accountDestiny = await GetAccount(dto.AccountDestinyNumber);

            Transactions transaction = new Transactions(dto);
            transaction.Id = Id + 1;
            transaction.Date = DateTime.Now;
            transaction.Destiny = accountDestiny;

            // Add transaction to list
            transactions.Add(transaction);

            var filter = Builders<Account>.Filter.Eq("Number", account.Number);
            var update = Builders<Account>.Update.Set("Extract", transactions);
            await _accountCollection.UpdateOneAsync(filter, update);
            return transaction;
        }

    }
}
