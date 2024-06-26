using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AgencyCustomerDTO
    {
        [Key]
        [StringLength(14)]
        public string Cpf { get; set; }
        public DateTime DtBirth { get; set; }
        public bool Restriction { get; set; }
    }
}
