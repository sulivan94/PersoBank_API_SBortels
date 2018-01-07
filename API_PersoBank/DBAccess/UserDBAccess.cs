using API_PersoBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_PersoBank.DBAccess
{
    public class UserDBAccess
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        public ApplicationUser FindByUserName(string userName)
        {
            try
            {
                return context.Users.Where(u => u.UserName.Equals(userName)).Single();
            }
            catch (InvalidOperationException exc)
            {
                return null;
            }
        }

        public List<ApplicationUser> FindLastRegistered(DateTime date)
        {
            try
            {
                return context.Users.Where(u => u.InscriptionDate > date).ToList();
            }
            catch (InvalidOperationException exc)
            {
                return null;
            }
        }
    }
}