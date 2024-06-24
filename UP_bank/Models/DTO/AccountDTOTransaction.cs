using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AccountDTOTransaction
    {
        [BsonId]
        public string Number { get; set; }

        public AccountDTOTransaction(Account account)
        {
            this.Number = account.Number;
        }
    }
}
