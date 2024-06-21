using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Address
    {
        [Key]
        public string ZipCode { get; set; }
        public string Number { get; set; }
        public string Street { get; set; }
        public string? Complement { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}
