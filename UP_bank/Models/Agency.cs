using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Agency
    {
        public string Number { get; set; }
        public Address Address { get; set; }
        [StringLength(19)]
        public string Cnpj { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public bool Restriction { get; set; }
    }
}
