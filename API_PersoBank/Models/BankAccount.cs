using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace API_PersoBank.Models
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }

        [Required]
        public decimal InitialAmount { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public bool Authorization { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}