using Models.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AgencyEmployee
    {
        [Key]
        [StringLength(14)]
        public string Cpf { get; set; }
        public string AgencyNumber { get; set; }

        public AgencyEmployee()
        {
            
        }

        public AgencyEmployee(DeletedAgencyEmployee deletedAgencyEmployee)
        {
            Cpf = deletedAgencyEmployee.Cpf;
        }
    }
}
