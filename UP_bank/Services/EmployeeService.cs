using Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmployeeService
    {
        //public readonly IAddress _api;

        /*
        public EmployeeService(IAddress api) 
        {
            _api = api;
        }
        */
        

        //CRUD METHODS

        /*
        public async Task<Address> GetAddress(string zipCode, string number)
        {
            Address? address = await _api.GetAddress(new AddressDTO { ZipCode = zipCode, Number = number });

            if (address == null)
                throw new Exception("CEP inválido.");

            return address;
        }
        */

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
                    AddressZipCode = employeeDTO.AddressZipCode,
                    AddressNumber = employeeDTO.AddressNumber,
                    Manager = employeeDTO.Manager,
                    Register = employeeDTO.Register
                };

                return employee;
            }
            catch { throw new Exception("Occurred an error while creating employee"); }
        }

        public async Task<Employee> UpdateEmployee(Employee employee, EmployeeDTO employeeDTO)
        {
            try
            {
                employee.Name = employeeDTO.Name;
                employee.DtBirth = employeeDTO.DtBirth;
                employee.Sex = employeeDTO.Sex;
                employee.Income = employeeDTO.Income;
                employee.Phone = employeeDTO.Phone;
                employee.Email = employeeDTO.Email;

                return employee;
            }
            catch { throw new Exception("Occurred an error while updating employee"); }
        }

        public async Task<Employee> DeleteEmployee(Employee employee)
        {

            DeletedEmployee deletedEmployee;
            try
            {
                deletedEmployee = new()
                {
                    Cpf = employee.Cpf,
                    Name = employee.Name,
                    DtBirth = employee.DtBirth,
                    Sex = employee.Sex,
                    Income = employee.Income,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    AddressZipCode = employee.AddressZipCode,
                    AddressNumber = employee.AddressNumber,
                    Manager = employee.Manager,
                    Register = employee.Register,
                };

                return deletedEmployee;

            }
            catch { throw new Exception("Occurred an error while deleting employee"); }
        }

        //

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
            catch (Exception ex) { return false; }
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
            catch (Exception ex) { return false; }
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
            catch (Exception ex) { return false; }
        }
        #endregion

        //ANOTHER METHODS
        public void AproveAccount()
        { }

        public void DefineAccountPerfil()
        { }
    }
}
