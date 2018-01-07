using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_PersoBank.DTO_s
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public string Comment { get; set; }
        public long TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public int? PlaceId { get; set; }
        public int AccountId { get; set; }
        public int? CategoryId { get; set; }
    }

    public class TransactionDetailDTO
    {
        public int TransactionId { get; set; }
        public string Comment { get; set; }
        public long TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public bool IsExpense { get; set; }
        public string Place { get; set; }
        public string User { get; set; }
        public string Category { get; set; }
    }

    public class LastTransactionDTO
    {
        public long Date { get; set; }
        public decimal Amount { get; set; }
    }
}