using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DeletedAgencyEmployee
    {
        [Key]
        [StringLength(14)]
        public string Cpf { get; set; }

        public DeletedAgencyEmployee()
        {
            
        }

        public DeletedAgencyEmployee(AgencyEmployee agencyEmployee)
        {
            Cpf = agencyEmployee.Cpf;
        }
    }
}
