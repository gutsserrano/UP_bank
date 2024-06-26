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

        public async Task<Account> GetAccount(string accNumber)
        {
            var account = await _accountCollection.Find(x => x.Number == accNumber).FirstOrDefaultAsync();
            return account;
        }

        public async Task<Transactions> GetExtractId(Account account, int id)
        {
            List<Transactions>? transactions = null;
            Transactions? transaction = null;

            transactions = account.Extract;

            if (transactions != null)
                transaction = transactions.Find(x => x.Id == id);

            return transaction;
        }

        public async Task<Transactions> GetExtractType(Account account, int type)
        {
            List<Transactions>? transactions = null;
            Transactions? transaction = null;

            transactions = account.Extract;

            if (transactions != null)
                transaction = transactions.Find(x => (int)x.Type == type);

            return transaction;
        }


        public async Task<List<Transactions>> GetExtract(string number)
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
            List<Transactions> transactionsList = new List<Transactions>();

            #region Account Origin
            // Load all existing extracts
            if (accountOrigin.Extract != null)
            {
                foreach (var item in accountOrigin.Extract)
                {
                    transactionsList.Add(item);
                    Id = item.Id;
                }
            }

            if (Type == 0 || Type == 3 || Type == 4)
            {
                dto.Price = dto.Price * -1;
            }

            // Set transaction values
            AccountTransactionDTO accountDTOTransaction = null;
            var accountDestiny = await GetAccount(dto.AccountDestinyNumber);
            if (accountDestiny != null)
                accountDTOTransaction = new AccountTransactionDTO(accountDestiny, ETransactionType.Sent);

            Transactions transactionOrigin = new Transactions(dto);
            transactionOrigin.Id = Id + 1;
            transactionOrigin.Date = DateTime.Now;
            transactionOrigin.Account = accountDTOTransaction;

            // Add transactionOrigin to list
            transactionsList.Add(transactionOrigin);

            var filter = Builders<Account>.Filter.Eq("Number", accountOrigin.Number);
            var update = Builders<Account>.Update.Set("Extract", transactionsList);
            await _accountCollection.UpdateOneAsync(filter, update);
            #endregion

            #region Account Destiny
            //////////// Account Destiny
            // Load all existing extracts
            if (accountDestiny != null)
            {
                transactionsList = new List<Transactions>();
                Id = 0;
                if (accountDestiny.Extract != null)
                {
                    foreach (var item in accountDestiny.Extract)
                    {
                        transactionsList.Add(item);
                        Id = item.Id;
                    }
                }

                // Set transaction values
                dto.Price = dto.Price * -1;
                Transactions transactionDestiny = new Transactions(dto);
                transactionDestiny.Id = Id + 1;
                transactionDestiny.Date = DateTime.Now;
                transactionDestiny.Account = new AccountTransactionDTO(accountOrigin, ETransactionType.Received);
                // Add transaction to list
                transactionsList.Add(transactionDestiny);

                var filterDestiny = Builders<Account>.Filter.Eq("Number", accountDestiny.Number);
                var updateDestiny = Builders<Account>.Update.Set("Extract", transactionsList);
                await _accountCollection.UpdateOneAsync(filterDestiny, updateDestiny);
            }
            #endregion


            // Return Origin Transaction
            return transactionOrigin;
        }

        public async Task<string> ValidateTransaction(Account account, TransactionsDTO dto)
        {
            int Type = (int)dto.Type;
            var accountDestiny = await GetAccount(dto.AccountDestinyNumber);

            if ((int)dto.Type < 0 || (int)dto.Type > 4) return "Transaction type does not exist!";

            if (Type != 3 && (accountDestiny != null || dto.AccountDestinyNumber != String.Empty)) return "Operation not allowed, you can only inform the Destiny Account in a Transfer type transaction!";

            if (Type == 0 || Type == 3 || Type == 4) // Subtract balance
            {
                if (accountDestiny == null && Type == 3) return "Destiny account not located!";
                if (dto.Price > (account.Balance + account.Overdraft)) return $"Your Account Balance is $ {account.Balance} and your Overdraft is $ {account.Overdraft}, with Total of $ {account.Balance + account.Overdraft} available. This value is lower than Transaction value of $ {dto.Price}!";
            }

            if (accountDestiny == null)
            {
                if (account.Agency.Restriction == true) return "Account Agency is restricted!";
                if (account.Restriction == true) return "Account Origin is restricted!";
                if (account.Customers[0].Restriction == true) return "Account Origin Customer is restricted!";
            }
            else
            {
                if (accountDestiny.Agency.Restriction == true) return "Account Destiny Agency is restricted!";
                if (accountDestiny.Restriction == true) return "Account Destiny is restricted!";
                if (accountDestiny.Customers[0].Restriction == true) return "Account Destiny Customer is restricted!";
                if (account.Number == accountDestiny.Number) return "You cannot transfer to your own account! Use Deposit instead.";
            }

            return "Ok";
        }
    }
}
