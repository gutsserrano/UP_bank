using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace UPBank.AddressAPI.Data
{
    public class UPBankAddressAPIContext : DbContext
    {
        public UPBankAddressAPIContext (DbContextOptions<UPBankAddressAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Address> Address { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Address>()
                .HasKey(a => new { a.ZipCode, a.Number });
        }
    }
}
