using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTO;

namespace Models
{
    public class Transactions
    {
        [BsonId]
        public int Id { get; set; }
        [BsonRequired]
        public DateTime Date { get; set; }
        [BsonRequired]
        [BsonRepresentation(BsonType.String)]         // Convert Enum to string in document
        public EType Type { get; set; }
        [BsonRequired]
        //public Account? Destiny { get; set; }
        public AccountTransactionDTO? Account { get; set; }
        [BsonRequired]
        public double Price { get; set; }

        public Transactions()
        {
            
        }

        public Transactions(TransactionsDTO dto)
        {
            this.Type = dto.Type;
            this.Price = dto.Price;
        }
    }
}
