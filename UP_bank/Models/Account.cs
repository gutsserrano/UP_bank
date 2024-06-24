using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Account
    {
        [BsonId]
        public string Number { get; set; }
        public Agency Agency { get; set; }
        public List<Customer> Customers { get; set; }
        public bool Restriction { get; set; }
        public CreditCard CreditCard { get; set; }
        public double Overdraft { get; set; }
        [BsonRepresentation(BsonType.String)]         // Convert Enum to string in document
        public EProfile Profile { get; set; }
        public DateTime Date { get; set; }
        public decimal Balance { get; set; }
        [JsonProperty("Extract")]
        public List<Transactions> Extract { get; set; }
    }
}
