using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class DeletedAgency
    {
        [Key]
        public string Number { get; set; }
        [StringLength(19)]
        public string Cnpj { get; set; }
        public bool Restriction { get; set; }
        [NotMapped]
        public List<Employee> Employees { get; set; } = new List<Employee>();
        [JsonIgnore]
        public List<DeletedAgencyEmployee> EmployeesCpf { get; set; } = new List<DeletedAgencyEmployee>();
        [NotMapped]
        public Address Address { get; set; }
        [JsonIgnore]
        [StringLength(9)]
        public string AddressZipCode { get; set; }
        [JsonIgnore]
        public string AddressNumber { get; set; }

        public DeletedAgency()
        {
            
        }

        public DeletedAgency(Agency agency)
        {
            List<DeletedAgencyEmployee> deleteds = new List<DeletedAgencyEmployee>();

            foreach(var item in agency.EmployeesCpf)
            {
                deleteds.Add(new DeletedAgencyEmployee(item));
            }

            Number = agency.Number;
            Cnpj = agency.Cnpj;
            Restriction = agency.Restriction;
            Employees = agency.Employees;
            EmployeesCpf = deleteds;
            Address = agency.Address;
            AddressZipCode = agency.AddressZipCode;
            AddressNumber = agency.AddressNumber;
        }
    }
}
