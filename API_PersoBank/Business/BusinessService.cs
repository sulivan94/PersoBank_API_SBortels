using API_PersoBank.DTO_s;
using API_PersoBank.Models;
using API_PersoBank.Util;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace API_PersoBank.Business
{
    public class BusinessService
    {
        public TransactionsDetail CalculTransactionsDetail(List<Transaction> transactionsList)
        {
            decimal totalAmount = GetTotalAmount(transactionsList);

            TransactionsDetail detail = new TransactionsDetail()
            {
                TotalAmount = totalAmount,
                NbTransactions = transactionsList.Count,
                AverageAmount = (transactionsList.Count != 0) ? totalAmount / transactionsList.Count : 0
            };
            return detail;
        }

        public bool IsCorrectVariationCategory(Category category, TransactionsDetail detail, int variationPourcentage)
        {
            if (category.AverageAmount == null && detail.AverageAmount > 0)
                return true;

            if (category.AverageAmount == null && detail.AverageAmount == 0)
                return false;

            return IsCorrectVariation(category.AverageAmount, detail.AverageAmount, variationPourcentage);
        }

        public bool IsCorrectVariationPlace(Place place, TransactionsDetail detail, int variationPourcentage)
        {
            if (place.AverageAmount == null && detail.AverageAmount > 0)
                return true;

            if (place.AverageAmount == null && detail.AverageAmount == 0)
                return false;

            return IsCorrectVariation((decimal)place.AverageAmount, detail.AverageAmount, variationPourcentage);
        }

        public bool IsCorrectVariation(decimal? currentAverage, decimal theoreticalAverage, int pourcentage)
        {
            if (currentAverage != null)
            {
                decimal maxAmount = (decimal)currentAverage * (1 + (decimal)pourcentage / 100);
                if (theoreticalAverage > maxAmount)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public decimal GetAccountBalance(List<Transaction> transactionsList, decimal initialAmount)
        {
            foreach(Transaction transaction in transactionsList)
            {
                if (transaction.Category.Expense)
                    initialAmount -= transaction.Amount;
                else
                    initialAmount += transaction.Amount;
            }
            return initialAmount;
        }

        public decimal GetTotalAmount(List<Transaction> transactionsList)
        {
            decimal amount = 0;
            foreach(Transaction t in transactionsList)
            {
                amount += t.Amount;
            }
            return amount;
        }

        public double getPourcentage(decimal totalAmount, decimal categoryAmount)
        {
            return ((double)categoryAmount / (double)totalAmount) * 100;
        }

        public List<LastTransactionDTO> InitializeLastWeekTransactionList()
        {
            List<LastTransactionDTO> list = new List<LastTransactionDTO>();

            for (int i = 0; i < 7; i++)
            {
                DateTime currentDate = DateTime.Now.AddDays(-i);
                list.Add(new LastTransactionDTO
                {
                    Date = DateConverter.DateTimeToLong(currentDate),
                    Amount = 0
                });
            }
            return list;
        }

        public List<LastTransactionDTO> UpdateLastWeekTransactionList(List<LastTransactionDTO> currentList, DateTime date, decimal amount)
        {
            List<LastTransactionDTO> list = currentList;

            for (int i = 0; i < 7; i++)
            {
                DateTime currentDate = DateConverter.DoubleToDateTime(list.ElementAt(i).Date);
                if (currentDate.CompareTo(date) == 0)
                {
                    list.ElementAt(i).Amount = amount;
                    break;
                }
            }
            return list;
        }
    }
}