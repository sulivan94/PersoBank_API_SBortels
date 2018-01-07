using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace API_PersoBank.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Budget>()
                .HasRequired(b => b.BankAccount)
                .WithMany()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Transaction>()
                .HasRequired(t => t.BankAccount)
                .WithMany()
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<BankAccount> BankAccounts { get; set; }

        public DbSet<Budget> Budgets { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Place> Places { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
    }
}