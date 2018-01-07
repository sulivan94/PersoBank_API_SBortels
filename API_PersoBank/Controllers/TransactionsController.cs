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
using API_PersoBank.DTO_s;

namespace API_PersoBank.Controllers
{
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private TransactionDBAccess _transactionDBAccess = new TransactionDBAccess();
        private BusinessService _businessService = new BusinessService();

        // GET: api/Transactions
        public IQueryable<Transaction> GetTransactions()
        {
            return context.Transactions;
        }

        // GET: api/Transactions/5
        [Route("api/Transactions/{id:int}")]
        [HttpGet]
        public IHttpActionResult GetTransaction(int id)
        {
            TransactionDTO transaction = _transactionDBAccess.FindById(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // PUT: api/Transactions/5
        [Route("api/Transactions/{id:int}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutTransaction([FromUri]int id, [FromBody]Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transaction.TransactionId)
            {
                return BadRequest();
            }

            context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return BadRequest("Unable to save changes, the transaction was deleted !");
                }
                else
                {
                    return BadRequest("The transaction you attempted to update was modified by another user after you got the original value, try again later !");
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Transactions
        [Route("api/Transactions")]
        [HttpPost]
        public async Task<IHttpActionResult> PostTransaction([FromBody]Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            return Created("PersoBankApi", transaction);
        }

        // DELETE: api/Transactions/5
        [Route("api/Transactions/{id:int}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteTransaction(int id)
        {
            Transaction transaction = await context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            context.Transactions.Remove(transaction);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return BadRequest("Unable to save changes, the transaction has already deleted !");
                }
                else
                {
                    return BadRequest("The transaction you attempted to delete was modified by another user after you got the original value, try again later !");
                }
            }
            return Ok(transaction);
        }

        [Route("~/api/categories/{categoryId:int}/transactionsDetail")]
        [HttpGet]
        public IHttpActionResult GetTransactionsDetailByCategory(int categoryId)
        {
            try
            {
                var transactions = _transactionDBAccess.FindByCategory(categoryId);
                var transactionsDetail = _businessService.CalculTransactionsDetail(transactions);
                return Ok(transactionsDetail);
            }
            catch (Exception exc)
            {
                return Ok(exc.Message);
            }
        }

        [Route("~/api/places/{placeId:int}/transactionsDetail")]
        [HttpGet]
        public IHttpActionResult GetTransactionsDetailByPlace(int placeId)
        {
            try
            {
                var transactions = _transactionDBAccess.FindByPlace(placeId);
                var transactionsDetail = _businessService.CalculTransactionsDetail(transactions);
                return Ok(transactionsDetail);
            }
            catch (Exception exc)
            {
                return Ok(exc.Message);
            }
        }

        [Route("api/Transactions/GraphicDetails")]
        [HttpPost]
        public IHttpActionResult GetAllGraphicDetails([FromBody]string userId)
        {
            Transaction[] transactionArray = _transactionDBAccess.FindExpensesByUser(userId);
            decimal totalAmountExpended = _businessService.GetTotalAmount(transactionArray.ToList());

            List<GraphicDetailDTO> graphicDetails = new List<GraphicDetailDTO>();
            string categoryName;
            decimal categoryAmount;
            int i = 0;
            while (i < transactionArray.Count())
            {
                categoryAmount = 0;
                categoryName = ((Transaction)transactionArray.GetValue(i)).Category.Label;
                while (i < transactionArray.Count() && categoryName.Equals(((Transaction)transactionArray.GetValue(i)).Category.Label))
                {
                    categoryAmount += ((Transaction)transactionArray.GetValue(i)).Amount;
                    i++;
                }
                graphicDetails.Add(new GraphicDetailDTO
                {
                    CategoryName = categoryName,
                    Amount = categoryAmount,
                    Pourcentage = _businessService.getPourcentage(totalAmountExpended, categoryAmount)
                });
            }
            return Ok(graphicDetails);
        }

        [Route("api/Transactions/GetLastTransactions")]
        [HttpPost]
        public IHttpActionResult GetLastTransactions([FromBody]string userId)
        {
            // Initialisation de la liste avec les dates des 7 derniers jours
            List<LastTransactionDTO> transactionList = _businessService.InitializeLastWeekTransactionList();

            Transaction[] transactionArray = _transactionDBAccess.FindLastWeekTransactions(userId);

            DateTime currentDate;
            decimal dateAmount;
            int i = 0;
            while (i < transactionArray.Count())
            {
                dateAmount = 0;
                currentDate = ((Transaction)transactionArray.GetValue(i)).TransactionDate;
                while (i < transactionArray.Count() && ((Transaction)transactionArray.GetValue(i)).TransactionDate.CompareTo(currentDate) == 0)
                {
                    dateAmount += ((Transaction)transactionArray.GetValue(i)).Amount;
                    i++;
                }
                // On ajoute le montant à la date correspondante dans la liste
                transactionList = _businessService.UpdateLastWeekTransactionList(transactionList, currentDate, dateAmount);
            }
            return Ok(transactionList);
        }

        [Route("api/Transactions/GetByAccount/{accountId:int}")]
        [HttpGet]
        public IHttpActionResult GetTransactionsByAccount([FromUri]int accountId)
        {
            List<TransactionDetailDTO> transactionList = _transactionDBAccess.FindByAccountDto(accountId);
            return Ok(transactionList);
        }

        [Route("api/Transactions/Expenses")]
        [HttpPost]
        public IHttpActionResult GetAllExpenses([FromBody]string userId)
        {
            List<TransactionDetailDTO> transactionList = _transactionDBAccess.FindUserExpenses(userId);
            return Ok(transactionList);
        }

        [Route("api/Transactions/Incomes")]
        [HttpPost]
        public IHttpActionResult GetAllIncomes([FromBody]string userId)
        {
            List<TransactionDetailDTO> transactionList = _transactionDBAccess.FindUserIncomes(userId);
            return Ok(transactionList);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionExists(int id)
        {
            return context.Transactions.Count(e => e.TransactionId == id) > 0;
        }
    }
}