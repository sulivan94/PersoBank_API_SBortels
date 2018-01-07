using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using API_PersoBank.Models;
using API_PersoBank.DBAccess;
using API_PersoBank.Business;
using API_PersoBank.Util;

namespace API_PersoBank.Controllers
{
    public class BankAccountsController : ApiController
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private AccountDBAccess _accountDBAccess = new AccountDBAccess();
        private TransactionDBAccess _transactionDBAccess = new TransactionDBAccess();
        private BusinessService _businessService = new BusinessService();

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public List<BankAccount> GetBankAccounts()
        {
            return context.BankAccounts.ToList();
        }

        // GET: api/BankAccounts/5
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBankAccount(int id)
        {
            BankAccount bankAccount = await context.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return Ok(bankAccount);
        }

        // PUT: api/BankAccounts/5
        [Authorize(Roles = "Admin, User")]
        [HttpPut]
        public async Task<IHttpActionResult> PutBankAccount(int id, BankAccount bankAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bankAccount.BankAccountId)
            {
                return BadRequest();
            }

            context.Entry(bankAccount).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
                {
                    return BadRequest("Unable to save changes, the account was deleted !");
                }
                else
                {
                    return BadRequest("The account you attempted to update was modified by another user after you got the original value, try again later !");
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/BankAccounts
        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public async Task<IHttpActionResult> PostBankAccount([FromBody]BankAccount bankAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            context.BankAccounts.Add(bankAccount);
            await context.SaveChangesAsync();

            return Created("PersoBankApi", bankAccount);
        }

        // DELETE: api/BankAccounts/5
        [Authorize(Roles = "Admin, User")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteBankAccount([FromUri]int id)
        {
            BankAccount bankAccount = await context.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            context.BankAccounts.Remove(bankAccount);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
                {
                    return BadRequest("Unable to save changes, the account has already deleted !");
                }
                else
                {
                    return BadRequest("The account you attempted to delete was modified by another user after you got the original value, try again later !");
                }
            }
            return Ok(bankAccount);
        }

        [Authorize(Roles = "Admin, User")]
        [Route("api/BankAccounts/GetUserAccounts")]
        [HttpPost]
        public List<AccountDetailDTO> GetUserAccounts([FromBody]String userId)
        {
            AccountDetailDTO accountDetail;
            List<AccountDetailDTO> accountDetailList = new List<AccountDetailDTO>();

            List<BankAccount> accountsList = _accountDBAccess.FindByUser(userId);
            foreach (BankAccount account in accountsList)
            {
                List<Transaction> accountTransactions = _transactionDBAccess.FindByAccount(account.BankAccountId);

                decimal balance = _businessService.GetAccountBalance(accountTransactions, account.InitialAmount);
                accountDetail = new AccountDetailDTO
                {
                    AccountId = account.BankAccountId,
                    Name = account.Name,
                    InitialAmount = account.InitialAmount,
                    Balance = balance,
                    LastTransactionDate = accountTransactions.Count != 0 ? DateConverter.DateTimeToLong(accountTransactions.Last().TransactionDate) : 0,
                    UserId = account.UserId
                };
                accountDetailList.Add(accountDetail);
            }
            return accountDetailList;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BankAccountExists(int id)
        {
            return context.BankAccounts.Count(e => e.BankAccountId == id) > 0;
        }
    }
}