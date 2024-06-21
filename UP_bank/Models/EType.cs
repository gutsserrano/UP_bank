using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum EType : int
    {
        Withdrawal = 0, 
        Loan = 1, 
        Deposit = 2, 
        Transfer = 3, 
        Payment = 4
    }
}
