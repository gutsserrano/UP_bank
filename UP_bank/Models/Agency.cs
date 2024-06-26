﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class Agency
    {
        [Key]
        public string Number { get; set; }
        [StringLength(19)]
        public string Cnpj { get; set; }
        public bool Restriction { get; set; }
        [NotMapped]
        public List<Employee> Employees { get; set; } = new List<Employee>();
        [JsonIgnore]
        public List<AgencyEmployee> EmployeesCpf { get; set; } = new List<AgencyEmployee>();
        [NotMapped]
        public Address Address { get; set; }
        [JsonIgnore]
        [StringLength(9)]
        public string AddressZipCode { get; set; }
        [JsonIgnore]
        public string AddressNumber { get; set; }

        public Agency()
        {
            
        }

        public Agency(DeletedAgency deletedAgency)
        {
            List<AgencyEmployee> employees = new List<AgencyEmployee>();
            foreach(var item in deletedAgency.EmployeesCpf)
            {
                employees.Add(new AgencyEmployee(item));
            }

            Number = deletedAgency.Number;
            Cnpj = deletedAgency.Cnpj;
            Restriction = deletedAgency.Restriction;
            Employees = deletedAgency.Employees;
            EmployeesCpf = employees;
            Address = deletedAgency.Address;
            AddressZipCode = deletedAgency.AddressZipCode;
            AddressNumber = deletedAgency.AddressNumber;
        }
    }
}
