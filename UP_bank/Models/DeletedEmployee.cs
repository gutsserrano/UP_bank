using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DeletedEmployee
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
        public bool Manager { get; set; }
        public int Register { get; set; }

        public DeletedEmployee() { }
        public DeletedEmployee(Employee employee)
        {
            this.Cpf = employee.Cpf;
            this.Name = employee.Name;
            this.DtBirth = employee.DtBirth;
            this.Sex = employee.Sex;
            this.Income = employee.Income;
            this.Email = employee.Email;
            this.Phone = employee.Phone;
            this.Address = employee.Address;
            this.AddressZipCode = employee.AddressZipCode;
            this.AddressNumber = employee.AddressNumber;
            this.Manager = employee.Manager;
            this.Register = employee.Register;
        }
    }
}
