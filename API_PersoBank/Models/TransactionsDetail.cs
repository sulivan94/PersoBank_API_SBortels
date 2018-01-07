using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_PersoBank.Models
{
    public class TransactionsDetail
    {
        public decimal TotalAmount { get; set; }
        public decimal AverageAmount { get; set; }
        public int NbTransactions { get; set; }
    }
}