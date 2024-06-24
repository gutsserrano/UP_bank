using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
namespace Models.DTO
{
    public class TransactionsDTO
    {
        public string AccountDestinyNumber { get; set; }
        public EType Type { get; set; }
        public double Price { get; set; }
    }
}
