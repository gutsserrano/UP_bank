using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace UP_bank.EmployeeAPI.Data
{
    public class UP_bankEmployeeAPIContext : DbContext
    {
        public UP_bankEmployeeAPIContext (DbContextOptions<UP_bankEmployeeAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Employee> Employee { get; set; } = default!;
        public DbSet<Models.DeletedEmployee> DeletedEmployee { get; set; } = default!;
    }
}
