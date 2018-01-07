using API_PersoBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_PersoBank.DBAccess
{
    public class PlaceDBAccess
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        public List<Place> FindAll()
        {
            return context.Places.ToList();
        }
    }
}