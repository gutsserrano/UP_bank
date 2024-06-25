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
        public DbSet<Models.AgencyEmployee> AgencyEmployee { get; set; } = default!;
        public DbSet<Models.DeletedAgency> DeletedAgency { get; set; } = default!;
        public DbSet<Models.DeletedAgencyEmployee> DeletedAgencyEmployee { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Agency>().ToTable("Agency");

            modelBuilder.Entity<AgencyEmployee>().ToTable("AgencyEmployee");

            modelBuilder.Entity<DeletedAgency>().ToTable("DeletedAgency");

            modelBuilder.Entity<DeletedAgencyEmployee>().ToTable("DeletedAgencyEmployee");
        }
    }
}
