using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AccountDTO
    {
        public string Agency { get; set; }
        public string OwnerCpf { get; set; }
        public string? DependentCpf { get; set; }
        public EProfile Profile { get; set; }
    }
}
