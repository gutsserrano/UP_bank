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
    public class EmployeeUpdateDTO
    {
        public string Cpf { get; set; }
        public string Name { get; set; }
        public DateTime DtBirth { get; set; }
        public char Sex { get; set; }
        public double Income { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Manager { get; set; }
    }
}
