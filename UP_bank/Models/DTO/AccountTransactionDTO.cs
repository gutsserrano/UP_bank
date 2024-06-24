using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AccountTransactionDTO
    {
        [BsonId]
        public string Number { get; set; }
        [BsonRepresentation(BsonType.String)]         // Convert Char to string
        public ETransactionType Type { get; set; }

        public AccountTransactionDTO(Account account, ETransactionType type)
        {
            this.Number = account.Number;
            this.Type = type;
        }
    }
}
