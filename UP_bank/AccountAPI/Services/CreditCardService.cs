using AccountAPI.Settings;
using Models;
using MongoDB.Driver;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using System.Transactions;

namespace AccountAPI.Services
{
    public class CreditCardService
    {
        private readonly IMongoCollection<Account> _accountCollection;

        public CreditCardService(IMongoDataSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _accountCollection = database.GetCollection<Account>(settings.AccountCollectionName);
        }
        public async Task<CreditCard> Get(Account account)
        {
            var creditCard = account.CreditCard;
            return creditCard;
        }

        public async Task<Account> GetAccount(string accNumber)
        {
            var account = await _accountCollection.Find(x => x.Number == accNumber).FirstOrDefaultAsync();
            return account;
        }

        public async Task<CreditCard> Post(string accNumber, CreditCard creditCard)
        {
            var filter = Builders<Account>.Filter.Eq("Number", accNumber);
            var update = Builders<Account>.Update.Set("CreditCard", creditCard);
            await _accountCollection.UpdateOneAsync(filter, update);

            return creditCard;
        }
    }
}
