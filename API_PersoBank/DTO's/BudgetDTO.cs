using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_PersoBank.DTO_s
{
    public class BudgetDTO
    {
        public int BudgetId { get; set; }
        public long BeginingDate { get; set; }
        public long EndDate { get; set; }
        public decimal Amount { get; set; }
        public decimal ExpendedAmount { get; set; }
        public string CategoryName { get; set; }
        public string AccountName { get; set; }
        public int AccountId { get; set; }
    }
}