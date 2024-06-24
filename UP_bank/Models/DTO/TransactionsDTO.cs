using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class TransactionsDTO
    {
        public string AccountDestinyNumber { get; set; }
        public EType Type { get; set; }
        public double Price { get; set; }
    }
}
