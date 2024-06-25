using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AccountBalanceDTO
    {
        public string Number { get; set; }
        public double Balance { get; set; }
        public double Overdraft { get; set; }
        public double Total { get; set; }

        public AccountBalanceDTO()
        {
            
        }

        public AccountBalanceDTO(Account account)
        {
            this.Number = account.Number;
            this.Balance = account.Balance;
            this.Overdraft = account.Overdraft;
            this.Total = account.Balance + account.Overdraft;
        }
    }
}
