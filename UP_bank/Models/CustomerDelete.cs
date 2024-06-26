using Models.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CustomerDelete
    {
        [Key]
        public string Cpf { get; set; }
        public string Name { get; set; }
        public DateTime DtBirth { get; set; }
        public char Sex { get; set; }
        public double Income { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AddressZipCode { get; set; }
        public string AddressNumber { get; set; }
        public bool Restriction { get; set; }

        // Construtor que aceita um objeto Customer e copia os dados
        public CustomerDelete(Customer customer)
        {
            Cpf = customer.Cpf;
            Name = customer.Name;
            DtBirth = customer.DtBirth;
            Sex = customer.Sex;
            Income = customer.Income;
            Phone = customer.Phone;
            Email = customer.Email;
            AddressZipCode = customer.AddressZipCode;
            AddressNumber = customer.AddressNumber;
            Restriction = customer.Restriction;
        }

        // Construtor padrão necessário para o Entity Framework
        public CustomerDelete() { }
    }

}
