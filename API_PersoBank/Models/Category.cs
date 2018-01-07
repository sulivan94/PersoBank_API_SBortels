using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace API_PersoBank.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Range(1,99999)]
        public decimal? AverageAmount { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Label { get; set; }

        [Required]
        public bool Expense { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

    public class BestCategory
    {
        public string Label { get; set; }
        public int NbTransactions { get; set; }
    }
}