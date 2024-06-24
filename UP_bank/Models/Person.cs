using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public abstract class Person
    {
        [Key]
        [StringLength(14)]
        public string Cpf { get; set; }
        public string Name { get; set; }
        public DateTime DtBirth { get; set; }
        [BsonRepresentation(BsonType.String)]         // Convert Char to string
        public char Sex { get; set; }
        public double Income { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [NotMapped]
        public Address Address { get; set; }
        //[JsonIgnore]
        [StringLength(9)]
        public string AddressZipCode { get; set; }
        //[JsonIgnore]
        public string AddressNumber { get; set; }
    }
}
