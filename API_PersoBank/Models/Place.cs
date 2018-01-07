using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace API_PersoBank.Models
{
    public class Place
    {
        public int PlaceId { get; set; }

        [Range(1,99999)]
        public decimal? AverageAmount { get; set; }

        [MinLength(2)]
        [MaxLength(100)]
        public string Street { get; set; }

        [Range(1,9999)]
        public int? StreetNumber { get; set; }

        [Range(1000, 9999)]
        public int? PostalCode { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(100)]
        public string Name { get; set; }

        // 1 lieu appartient à 1 catégorie
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

    public class BestPlace
    {
        public string Name { get; set; }
        public int NbTransactions { get; set; }
    }
}