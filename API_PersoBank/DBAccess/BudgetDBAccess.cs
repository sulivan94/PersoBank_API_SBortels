using API_PersoBank.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace API_PersoBank.DBAccess
{
    public class BudgetDBAccess
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        public List<Budget> FindByUser(string userId)
        {
            return context.Budgets
                .Include(b => b.Category)
                .Include(b => b.BankAccount)
                .Where(b => b.BankAccount.UserId.Equals(userId))
                .ToList();
        }
    }
}