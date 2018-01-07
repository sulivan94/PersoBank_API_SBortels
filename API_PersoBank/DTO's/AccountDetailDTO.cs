using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_PersoBank.Models
{
    public class AccountDetailDTO
    {
        public int AccountId { get; set; }
        public String Name { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal Balance { get; set; }
        public long? LastTransactionDate { get; set; }
        public string UserId { get; set; }
    }
}