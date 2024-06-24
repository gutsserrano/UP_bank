using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AgencyServices
{
    public class AgencyService
    {
        public async Task<List<Employee>> GetEmployees(List<AgencyEmployee> agencyEmployees)
        {
            // Este método deve buscar os funcionarios cadastrados a partir da lista de cpf passados como argumento
            Random random = new Random();
            int count = random.Next(1, 6);

            List<Employee> employees = new List<Employee>();
            for(int i = 0; i < count; i++)
            {
                employees.Add(GenerateRandomEmployee());
            }

            return employees;
        }

        public async Task<Address> GetAddress(string zipCode, string number)
        {
            // Este método deve buscar o endereço cadastrado a partir do cep e número passados como argumento
            return GenerateRandomAddress();
        }

        private Employee GenerateRandomEmployee()
        {
            Address address = GenerateRandomAddress();
            Random random = new Random();

            return new Employee()
            {
                Name = "Funcionario " + random.Next(1, 100),
                Cpf = random.Next(100000000, 999999999).ToString(),
                DtBirth = DateTime.Now,
                Sex = random.Next(0, 2) == 1 ? 'M' : 'F',
                Income = random.Next(1000, 10000),
                Phone = random.Next(100000000, 999999999).ToString(),
                Email = "funcionario" + random.Next(1, 100) + "@upbank.com",
                Address = address,
                AddressZipCode = address.ZipCode,
                AddressNumber = address.Number,
                Manager = random.Next(0, 2) == 1,
                Register = random.Next(1, 999)
            };
        }

        private Address GenerateRandomAddress()
        {
            Random random = new Random();

            return new Address()
            {
                Street = "Rua " + random.Next(1, 100),
                Number = random.Next(1, 1000).ToString(),
                Complement = "Complemento " + random.Next(1, 100),
                City = "Cidade " + random.Next(1, 100),
                State = "Estado " + random.Next(1, 100),
                ZipCode = random.Next(10000000, 99999999).ToString()
            };
        }
    }
}
