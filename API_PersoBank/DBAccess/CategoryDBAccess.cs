using API_PersoBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_PersoBank.DBAccess
{
    public class CategoryDBAccess
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        public List<Category> FindAll()
        {
            return context.Categories.ToList();
        }
    }
}