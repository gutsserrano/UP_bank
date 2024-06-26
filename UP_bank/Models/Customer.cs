using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Customer : Person
    {
        public bool Restriction { get; set; }

        public Customer()
        {
           
        }

        public Customer(CustomerUpdateDTO customerUpdateDTO)
        {
            Cpf = customerUpdateDTO.Cpf;
            Name = customerUpdateDTO.Name;
            DtBirth = customerUpdateDTO.DtBirth;
            Sex = customerUpdateDTO.Sex;
            Income = customerUpdateDTO.Income;
            Email = customerUpdateDTO.Email;
            Phone = customerUpdateDTO.Phone;
        }
    }


}
