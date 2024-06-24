﻿using MongoDB.Bson.Serialization.Attributes;
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
        [BsonId]
        public int Id { get; set; }
        [BsonRequired]
        public DateTime Date { get; set; }
        [BsonRequired]
        [BsonRepresentation(BsonType.String)]         // Convert Enum to string in document
        public EType Type { get; set; }
        [BsonRequired]
        public Account? Destiny { get; set; }
        [BsonRequired]
        public double Price { get; set; }
    }
}
