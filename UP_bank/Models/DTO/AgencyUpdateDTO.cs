using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AgencyUpdateDTO
    {
        public string Number { get; set; }
        public bool Restriction { get; set; }
    }
}
