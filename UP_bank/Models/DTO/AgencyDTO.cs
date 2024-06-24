using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AgencyDTO
    {
        public string Number { get; set; }
        [StringLength(19)]
        public string Cnpj { get; set; }
        public List<AgencyEmployee> EmployeesCpf { get; set; } = new List<AgencyEmployee>();
        public bool Restriction { get; set; }
        public AddressDTO AddressDTO { get; set; }
    }
}
