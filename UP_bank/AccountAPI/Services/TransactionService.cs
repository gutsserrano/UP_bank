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

        public async Task<Transactions> CreateTransaction(Account accountOrigin, TransactionsDTO dto)
        {
            int Id = 0;
            int Type = (int)dto.Type;
            List<Transactions> transactions = new List<Transactions>();

            #region Account Origin
            // Load all existing extracts
            if (accountOrigin.Extract != null)
            {
                foreach (var item in accountOrigin.Extract)
                {
                    transactions.Add(item);
                    Id = item.Id;
                }
            }

            if (Type == 0 || Type == 3 || Type == 4)
            {
                dto.Price = dto.Price * -1;
            }

            // Set transaction values
            AccountDTOTransaction accountDTOTransaction = null;
            var accountDestiny = await GetAccount(dto.AccountDestinyNumber);
            if (accountDestiny != null)
                accountDTOTransaction = new AccountDTOTransaction(accountDestiny);

            Transactions transaction = new Transactions(dto);
            transaction.Id = Id + 1;
            transaction.Date = DateTime.Now;
            transaction.Destiny = accountDTOTransaction;

            // Add transaction to list
            transactions.Add(transaction);

            var filter = Builders<Account>.Filter.Eq("Number", accountOrigin.Number);
            var update = Builders<Account>.Update.Set("Extract", transactions);
            await _accountCollection.UpdateOneAsync(filter, update);
            #endregion

            #region Account Destiny
            //////////// Account Destiny
            // Load all existing extracts
            if (accountDestiny != null)
            {
                transactions = new List<Transactions>();
                Id = 0;
                if (accountDestiny.Extract != null)
                {
                    foreach (var item in accountDestiny.Extract)
                    {
                        transactions.Add(item);
                        Id = item.Id;
                    }
                }

                // Set transaction values
                dto.Price = dto.Price * -1;
                Transactions transaction2 = new Transactions(dto);
                transaction2.Id = Id + 1;
                transaction2.Date = DateTime.Now;
                transaction2.Destiny = new AccountDTOTransaction(accountOrigin);
                // Add transaction to list
                transactions.Add(transaction2);

                var filterDestiny = Builders<Account>.Filter.Eq("Number", accountDestiny.Number);
                var updateDestiny = Builders<Account>.Update.Set("Extract", transactions);
                await _accountCollection.UpdateOneAsync(filterDestiny, updateDestiny);
            }
            #endregion


            // Return Origin Transaction
            return transaction;
        }
    }
}
