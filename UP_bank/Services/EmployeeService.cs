using Models;
using Models.DTO;
using MongoDB.Driver;
using Newtonsoft.Json;
using Services.AddressApiServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmployeeService
    {
        public readonly UPBankAddressApi _api;
        private readonly HttpClient _httpClient = new HttpClient();
        public EmployeeService(UPBankAddressApi api)
        {
            _api = api;
        }



        //CRUD METHODS
        #region Crud Methods
        public async Task<Address> GetAddress(string zipCode, string number)
        {
            Address? address = await _api.GetAddress(new AddressDTO { ZipCode = zipCode, Number = number });

            if (address == null)
                throw new Exception("CEP inválido.");

            return address;
        }


        public async Task<Address> GetAddressPostMethod(AddressDTO addressDTO)
        {
            Address? address = await _api.GetAddress(addressDTO);

            if (address == null)
                address = await _api.CreateAddress(addressDTO);

            if (address == null)
                throw new Exception("Invalid Zip Code!");

            return address;
        }

        public async Task<Employee> CreateEmployee(EmployeeDTO employeeDTO, Address address)
        {
            try
            {
                if (!ValidateCPF(employeeDTO.Cpf)) throw new Exception("Invalid CPF.");

                var employee = new Employee()
                {
                    Cpf = employeeDTO.Cpf,
                    Name = employeeDTO.Name,
                    DtBirth = employeeDTO.DtBirth,
                    Sex = employeeDTO.Sex,
                    Income = employeeDTO.Income,
                    Phone = employeeDTO.Phone,
                    Email = employeeDTO.Email,
                    Address = address,
                    Manager = employeeDTO.Manager,
                    Register = employeeDTO.Register,
                    AddressZipCode = address.ZipCode,
                    AddressNumber = address.Number
                };

                return employee;
            }
            catch (Exception) { throw new Exception("Occurred an error while creating employee"); }
        }

        public Employee UpdateEmployee(Employee employee, EmployeeUpdateDTO employeeUpdateDTO)
        {
            try
            {
                employee.Cpf = employeeUpdateDTO.Cpf;
                employee.Name = employeeUpdateDTO.Name;
                employee.DtBirth = employeeUpdateDTO.DtBirth;
                employee.Sex = employeeUpdateDTO.Sex;
                employee.Income = employeeUpdateDTO.Income;
                employee.Phone = employeeUpdateDTO.Phone;
                employee.Email = employeeUpdateDTO.Email;
                employee.Manager = employeeUpdateDTO.Manager;

                return employee;
            }
            catch { throw new Exception("Occurred an error while updating employee"); }
        }

        //Validate CPF
        #region Validate CPF
        public static bool ValidateCPF(string cpf)
        {
            try
            {
                if (cpf.Contains(".")) cpf = cpf.Replace(".", "");
                if (cpf.Contains("-")) cpf = cpf.Replace("-", "");

                if (cpf.Length != 11) return false;

                bool valid = false;

                for (int i = 0; i < cpf.Length - 1 && !valid; i++)
                {
                    int n1 = int.Parse(cpf.Substring(i, 1));
                    int n2 = int.Parse(cpf.Substring(i + 1, 1));

                    if (n1 != n2) valid = true;
                }

                return valid && ValidateFirstDigit(cpf) && ValidateSecondDigit(cpf);
            }
            catch (Exception) { return false; }
        }

        public static bool ValidateFirstDigit(string cpf)
        {
            try
            {
                int result = 0;

                for (int i = 0, mult = 10; i < 9; i++, mult--)
                {
                    result += int.Parse(cpf.Substring(i, 1)) * mult;
                }

                int rest = (result * 10) % 11;
                if (rest == 10) rest = 0;

                int firstDigit = int.Parse(cpf.Substring(9, 1));

                if (rest == firstDigit) return true;
                return false;
            }
            catch (Exception) { return false; }
        }

        public static bool ValidateSecondDigit(string cpf)
        {
            try
            {
                int result = 0;

                for (int i = 0, mult = 11; i < 10; i++, mult--)
                {
                    result += int.Parse(cpf.Substring(i, 1)) * mult;
                }

                int rest = (result * 10) % 11;

                int secondDigit = int.Parse(cpf.Substring(10, 1));

                if (rest == secondDigit) return true;
                return false;
            }
            catch (Exception) { return false; }
        }
        #endregion

        #endregion

        public async Task<Account> PutAproveAccount(string accNumber, bool restriction)
        {
            Account account = null;

            try
            {
                AccountRestrictionDTO dto = new AccountRestrictionDTO { Restriction = restriction };

                StringContent content = new(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

                var response = _httpClient.PatchAsync($"https://localhost:7244/api/accounts/account/{accNumber}", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    account = JsonConvert.DeserializeObject<Account>(response.Content.ReadAsStringAsync().Result);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return account;
        }

        public async Task<Account> GetAccount(string accNumber)
        {
            Account account = null;

            try
            {
                var response = _httpClient.GetAsync($"https://localhost:7244/api/accounts/account/{accNumber}?deleted=false").Result;
                if (response.IsSuccessStatusCode)
                {
                    account = JsonConvert.DeserializeObject<Account>(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    response = _httpClient.GetAsync($"https://localhost:7244/api/accounts/account/{accNumber}?deleted=true").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        account = JsonConvert.DeserializeObject<Account>(response.Content.ReadAsStringAsync().Result);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

            return account;
        }

        public async Task<Account> DefineAccountPerfil(AccountDTO accountDTO)
        {
            Account account = null;

            try
            {
                StringContent content = new(JsonConvert.SerializeObject(accountDTO), Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsync($"https://localhost:7244/api/accounts/", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    account = JsonConvert.DeserializeObject<Account>(response.Content.ReadAsStringAsync().Result);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return account;
        }
    }
}
