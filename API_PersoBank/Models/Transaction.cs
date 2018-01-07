using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace API_PersoBank.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        [MaxLength(250)]
        public string Comment { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [Range(0.20,99999)]
        public decimal Amount { get; set; }

        // 1 transaction correspond à 0 ou 1 lieu
        public int? PlaceId { get; set; }
        [ForeignKey("PlaceId")]
        public Place Place { get; set; }

        // 1 transaction correspond à 1 compte
        [Required]
        public int BankAccountId { get; set; }
        [ForeignKey("BankAccountId")]
        public BankAccount BankAccount { get; set; }

        // 1 transaction correspond à 1 catégorie
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}