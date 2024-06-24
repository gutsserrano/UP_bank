using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace UPBank.AgencyAPI.Data
{
    public class UPBankAgencyAPIContext : DbContext
    {
        public UPBankAgencyAPIContext (DbContextOptions<UPBankAgencyAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Agency> Agency { get; set; } = default!;
    }
}
