using AccountAPI.Settings;
using Models;
using MongoDB.Driver;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using System.Transactions;
using Newtonsoft.Json;
using Models.DTO;

namespace AccountAPI.Services
{
    public class CreditCardService
    {
        private readonly IMongoCollection<Account> _accountCollection;
        private readonly HttpClient _httpClient = new HttpClient();

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

        public async Task<CreditCard> Post(List<Customer> customers, Account account)
        {
            var accNumber = account.Number;
            CreditCard creditCard;
            bool ok = false;
            var cpfs = account.Customers.Select(c => c.Cpf).ToList();
            /*List<Customer> customers = new List<Customer>();

            for (int i = 0; i < cpfs.Count; i++)
            {
                try
                {
                    var response = _httpClient.GetAsync($"https://localhost:7147/api/Customers/{cpfs[i]}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var customer = JsonConvert.DeserializeObject<Customer>(response.Content.ReadAsStringAsync().Result);
                        customers.Add(customer);
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }*/

            do
            {
                creditCard = new CreditCard(customers.FirstOrDefault().Name, account.Profile.ToString());
                var x = await _accountCollection.Find(x => x.CreditCard.Number == creditCard.Number).FirstOrDefaultAsync();
                if (x == null)
                    ok = true;
            } while (!ok);

            var filter = Builders<Account>.Filter.Eq("Number", accNumber);
            var update = Builders<Account>.Update.Set("CreditCard", creditCard);
            await _accountCollection.UpdateOneAsync(filter, update);

            return creditCard;
        }
        public async Task<CreditCard> ActiveCard(string account, long card)
        {

            var acc = await _accountCollection.Find(x => x.Number == account && x.CreditCard.Number == card).FirstOrDefaultAsync();

            if (acc == null)
                throw new ArgumentException("Account or card not found!");

            if (acc.Restriction)
                throw new ArgumentException("Account is restricted!");

            if (acc.CreditCard.Active)
                throw new ArgumentException("Card is already active!");

            CreditCard creditCard = acc.CreditCard;
            var filter = Builders<Account>.Filter.Eq("Number", acc.Number);
            var update = Builders<Account>.Update.Set("CreditCard.Active", true);
            await _accountCollection.UpdateOneAsync(filter, update);
            creditCard.Active = true;

            return creditCard;
        }




    }
}
