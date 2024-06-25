using Models;
using Models.DTO;
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

        public EmployeeService(UPBankAddressApi api) 
        {
            _api = api;
        }
        
        

        //CRUD METHODS

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
            catch (Exception){ throw new Exception("Occurred an error while creating employee"); }
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

        //ANOTHER METHODS
        public void AproveAccount()
        { }

        public void DefineAccountPerfil()
        { }
    }
}
