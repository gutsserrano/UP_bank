using Models;
using Models.DTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace APICustomer.Services
{
    public class CustomerServices
    {

        public Customer UpdateCustomer(Customer customer, CustomerUpdateDTO customerUpdateDTO)
        {
            try
            {
                customer.Cpf = customerUpdateDTO.Cpf;
                customer.Name = customerUpdateDTO.Name;
                customer.DtBirth = customerUpdateDTO.DtBirth;
                customer.Sex = customerUpdateDTO.Sex;
                customer.Income = customerUpdateDTO.Income;
                customer.Phone = customerUpdateDTO.Phone;
                customer.Email = customerUpdateDTO.Email;
                customer.Restriction = customerUpdateDTO.Restriction;

                return customer;
            }
            catch { throw new Exception("Occurred an error while updating customer"); }
        }

        public  bool VerifyCpf(string cpf)
        {
            if (cpf.Contains(".") || cpf.Contains("-"))
            {
                cpf = cpf.Replace(".", "");
                cpf = cpf.Replace("-", "");
            }

            // Verifica se o tamanho do cpf é diferente de 11
            if (cpf.Length != 11)
            {
                return false;
            }

            bool valid = false;
            for (int i = 0; i < cpf.Length - 1 && !valid; i++)
            {
                int n1 = int.Parse(cpf.Substring(i, 1));
                int n2 = int.Parse(cpf.Substring(i + 1, 1));

                if (n1 != n2)
                {
                    valid = true;
                }
            }

            return valid && ValidationFirstDigit(cpf) && ValidationSecondDigit(cpf);
        }

        private bool ValidationFirstDigit(string str)
        {
            int result = 0;
            for (int i = 0, multiply = 10; i < 9; i++, multiply--)
            {
                int digit = int.Parse(str.Substring(i, 1));
                result += digit * multiply;
            }

            int rest = (result * 10) % 11;
            if (rest == 10)
                rest = 0;

            int digitOne = int.Parse(str.Substring(9, 1));

            if (rest == digitOne)
            {
                return true;
            }

            return false;
        }

        private bool ValidationSecondDigit(string str)
        {
            int result = 0;
            for (int i = 0, multiply = 11; i < 10; i++, multiply--)
            {
                int digit = int.Parse(str.Substring(i, 1));
                result += digit * multiply;
            }

            int rest = (result * 10) % 11;

            int digit2 = int.Parse(str.Substring(10, 1));

            if (rest == digit2)
            {
                return true;
            }

            return false;
        }
        public string RemoveMask(string cpf)
        {
            cpf = cpf.Replace(".", "");
            cpf = cpf.Replace("-", "");
            return cpf;
        }
        public string InsertMask(string cpf)
        {
            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }      
    }
}