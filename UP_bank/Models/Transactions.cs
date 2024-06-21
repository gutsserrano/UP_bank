using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Transactions
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [BsonRepresentation(BsonType.String)]         // Convert Enum to string in document
        public EType Type { get; set; }
        public Account? Destiny { get; set; }
        public double Price { get; set; }
    }
}
