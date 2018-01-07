using API_PersoBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_PersoBank.DBAccess
{
    public class AccountDBAccess
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        public List<BankAccount> FindByUser(string userId)
        {
            return context.BankAccounts
                .Where(a => a.UserId == userId)
                .ToList();
        }
    }
}