using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace API_PersoBank.Models
{
    public class Budget
    {
        public int BudgetId { get; set; }

        [Required]
        public DateTime BeginingDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1,99999)]
        public decimal Amount { get; set; }

        // 1 budget correspond à 1 compte
        [Required]
        public int BankAccountId { get; set; }
        [ForeignKey("BankAccountId")]
        public BankAccount BankAccount { get; set; }

        // 1 budget correspond à une catégorie
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}