using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace APICustomer.Data
{
    public class APICustomerContext : DbContext
    {
        public APICustomerContext (DbContextOptions<APICustomerContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Customer> Customer { get; set; } = default!;
    }
}
