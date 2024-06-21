using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Account
    {
        public string Number { get; set; }
        public Agency Agency { get; set; }
        public List<Customer> Customers { get; set; }
        public bool Restriction { get; set; }
        public CreditCard CreditCard { get; set; }
        public double Overdraft { get; set; }
        public EProfile Profile { get; set; }
        public DateTime Date { get; set; }
        public decimal Balance { get; set; }
        public List<Transactions> Extract { get; set; }
    }
}
