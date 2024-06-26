using Models;
using Models.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AgencyServices
{
    public class AgencyService
    {
        private readonly HttpClient _client = new HttpClient();

        public async Task<List<Employee>> GetEmployees(List<AgencyEmployee> agencyEmployees)
        {
            List<Employee>? employees = new();
            HttpResponseMessage response = new();

            foreach (var item in agencyEmployees)
            {
                var correctCpf = RemoveCpfMask(item.Cpf);
                string employeesApiUrl = $"https://localhost:7106/api/Employees/{correctCpf}";
                response = await _client.GetAsync(employeesApiUrl);

                string jsonResponse = await response.Content.ReadAsStringAsync();
                employees.Add(JsonConvert.DeserializeObject<Employee>(jsonResponse));
            }

            if ((int)response.StatusCode == 404)
                return null;

            if (employees == null)
                return null;

            return employees;
        }

        public async Task<Employee> GetEmployee(string cpf)
        {
            Employee? employee = new();
            HttpResponseMessage response = new();

            var correctCpf = RemoveCpfMask(cpf);
            string employeesApiUrl = $"https://localhost:7106/api/Employees/{correctCpf}";
            response = await _client.GetAsync(employeesApiUrl);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            employee = JsonConvert.DeserializeObject<Employee>(jsonResponse);
            

            if ((int)response.StatusCode == 404)
                return null;

            if (employee == null)
                return null;

            return employee;
        }

        public string RemoveCpfMask(string cpf)
        {
            cpf = cpf.Replace(".", "");
            cpf = cpf.Replace("-", "");
            return cpf;
        }

        public string InsertCpfMask(string cpf)
        {
            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }

        public string RemoveCnpjMask(string cnpj)
        {
            cnpj = cnpj.Replace(".", "");
            cnpj = cnpj.Replace("/", "");
            cnpj = cnpj.Replace("-", "");
            return cnpj;
        }

        public string InsertCnpjMask(string cnpj)
        {
            return Convert.ToUInt64(cnpj).ToString(@"00\.000\.000\/0000\-00");
        }

        public bool VerifyCnpj(string cnpj)
        {
            if (cnpj.Contains(".") || cnpj.Contains("/") || cnpj.Contains("-"))
            {
                cnpj = cnpj.Replace(".", "");
                cnpj = cnpj.Replace("/", "");
                cnpj = cnpj.Replace("-", "");
            }

            if (cnpj.Length != 14)
            {
                return false;
            }

            bool valido = false;
            for (int i = 0; i < cnpj.Length - 1 && !valido; i++)
            {
                int n1 = int.Parse(cnpj.Substring(i, 1));
                int n2 = int.Parse(cnpj.Substring(i + 1, 1));

                if (n1 != n2)
                {
                    valido = true;
                }
            }

            return valido && ValidateFirstDigit(cnpj) && ValidateSecondDigit(cnpj);
        }

        private bool ValidateFirstDigit(string str)
        {
            int result = 0;
            int[] verifier = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            for (int i = 0; i < 12; i++)
            {
                int digit = int.Parse(str.Substring(i, 1));
                result += digit * verifier[i];
            }

            int rest = result % 11;
            int firstDigit = int.Parse(str.Substring(12, 1));

            if ((rest == 0 || rest == 1) && firstDigit == 0)
            {
                return true;
            }
            else if ((rest >= 2 && rest <= 10) && firstDigit == 11 - rest)
            {
                return true;
            }

            return false;
        }

        private bool ValidateSecondDigit(string cnpj)
        {
            int result = 0;
            int[] verifier = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            for (int i = 0; i < 13; i++)
            {
                int digit = int.Parse(cnpj.Substring(i, 1));
                result += digit * verifier[i];
            }

            int rest = result % 11;
            int secondDigit = int.Parse(cnpj.Substring(13, 1));

            if ((rest == 0 || rest == 1) && secondDigit == 0)
            {
                return true;
            }
            else if ((rest >= 2 && rest <= 10) && secondDigit == 11 - rest)
            {
                return true;
            }

            return false;
        }
    }
}
