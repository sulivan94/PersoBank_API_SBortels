using API_PersoBank.DTO_s;
using API_PersoBank.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace API_PersoBank.DBAccess
{
    public class TransactionDBAccess
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        private static readonly Expression<Func<Transaction, TransactionDetailDTO>> AsTransactionDetailDto = 
            x => new TransactionDetailDTO
            {
                TransactionId = x.TransactionId,
                Comment = x.Comment,
                TransactionDate = 1000 * (long)DbFunctions.DiffSeconds(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), x.TransactionDate),
                Amount = x.Amount,
                IsExpense = x.Category.Expense,
                Place = x.Place.Name,
                User = x.BankAccount.Name,
                Category = x.Category.Label
            };

        private static readonly Expression<Func<Transaction, TransactionDTO>> AsTransactionDto =
            x => new TransactionDTO
            {
                TransactionId = x.TransactionId,
                Comment = x.Comment,
                TransactionDate = 1000 * (long)DbFunctions.DiffSeconds(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), x.TransactionDate),
                Amount = x.Amount,
                PlaceId = x.PlaceId,
                AccountId = x.BankAccountId,
                CategoryId = x.CategoryId
            };

        public TransactionDTO FindById(int id)
        {
            return context.Transactions
                .Where(t => t.TransactionId == id)
                .Select(AsTransactionDto)
                .FirstOrDefault();
        }

        public Transaction[] FindExpensesByUser(string userId)
        {
            try
            {
                return context.Transactions
                    .Include(t => t.Category)
                    .Where(t => t.BankAccount.UserId == userId && t.Category.Expense)
                    .OrderBy(t => t.CategoryId).ToArray();
            }
            catch (InvalidOperationException exc)
            {
                return null;
            }
        }

        public Transaction[] FindLastWeekTransactions(string userId)
        {
            Transaction[] transactions = FindExpensesByUser(userId);

            DateTime firstDate = DateTime.Now.AddDays(-6);
            DateTime lastDate = DateTime.Now;
            return transactions
                .Where(t => t.TransactionDate >= firstDate && t.TransactionDate <= lastDate)
                .OrderBy(t => t.TransactionDate)
                .ToArray();
        }

        public List<Transaction> FindByCategory(int categoryId)
        {
            try
            {
                return context.Transactions
                    .Where(t => t.CategoryId == categoryId)
                    .ToList();
            }
            catch (InvalidOperationException exc)
            {
                return null;
            }
        }

        public List<Transaction> FindByAccount(int accountId)
        {
            return context.Transactions
                .Where(t => t.BankAccountId == accountId)
                .ToList();
        }

        public List<TransactionDetailDTO> FindByAccountDto(int accountId)
        {
            return context.Transactions
                .Include(t => t.Place)
                .Include(t => t.BankAccount)
                .Include(t => t.Category)
                .Where(t => t.BankAccountId == accountId)
                .Select(AsTransactionDetailDto)
                .ToList();
        }

        public List<TransactionDetailDTO> FindUserExpenses(string userId)
        {
            return context.Transactions
                .Include(t => t.Place)
                .Include(t => t.BankAccount)
                .Include(t => t.Category)
                .Where(t => t.BankAccount.UserId.Equals(userId))
                .Where(t => t.Category.Expense == true)
                .Select(AsTransactionDetailDto)
                .ToList();
        }

        public List<TransactionDetailDTO> FindUserIncomes(string userId)
        {
            return context.Transactions
                .Include(t => t.Place)
                .Include(t => t.BankAccount)
                .Include(t => t.Category)
                .Where(t => t.BankAccount.UserId.Equals(userId))
                .Where(t => t.Category.Expense == false)
                .Select(AsTransactionDetailDto)
                .ToList();
        }

        public List<Transaction> FindByCategoryAndDate(int categoryId, DateTime date)
        {
            try
            {
                return context.Transactions
                    .Where(t => t.CategoryId == categoryId && t.TransactionDate > date)
                    .ToList();
            }
            catch (InvalidOperationException exc)
            {
                return null;
            }
        }

        public List<Transaction> FindByPlace(int placeId)
        {
            try
            {
                return context.Transactions
                    .Where(t => t.PlaceId == placeId)
                    .ToList();
            }
            catch (InvalidOperationException exc)
            {
                return null;
            }
        }

        public List<Transaction> FindByPlaceAndDate(int placeId, DateTime date)
        {
            try
            {
                return context.Transactions
                    .Where(t => t.PlaceId == placeId && t.TransactionDate > date)
                    .ToList();
            }
            catch (InvalidOperationException exc)
            {
                return null;
            }
        }

        public List<Transaction> FindAllBudgetTransactions(Budget budget)
        {
            return context.Transactions
                    .Where(t => t.BankAccount.UserId == budget.BankAccount.UserId)
                    .Where(t => t.CategoryId == budget.CategoryId)
                    .Where(t => t.TransactionDate >= budget.BeginingDate && t.TransactionDate <= budget.EndDate).ToList();
        }
    }
}