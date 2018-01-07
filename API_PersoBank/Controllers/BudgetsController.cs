using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using API_PersoBank.Models;
using API_PersoBank.DTO_s;
using API_PersoBank.Business;
using API_PersoBank.Util;
using API_PersoBank.DBAccess;

namespace API_PersoBank.Controllers
{
    public class BudgetsController : ApiController
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private BudgetDBAccess _budgetDBAccess = new BudgetDBAccess();
        private TransactionDBAccess _transactionDBAccess = new TransactionDBAccess();
        private BusinessService _businessService = new BusinessService();

        // GET: api/Budgets
        [Authorize(Roles = "Admin, User")]
        public IQueryable<Budget> GetBudgets()
        {
            return context.Budgets;
        }

        // GET: api/Budgets/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IHttpActionResult> GetBudget(int id)
        {
            Budget budget = await context.Budgets.FindAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            return Ok(budget);
        }

        // PUT: api/Budgets/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IHttpActionResult> PutBudget(int id, Budget budget)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != budget.BudgetId)
            {
                return BadRequest();
            }

            context.Entry(budget).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BudgetExists(id))
                {
                    return BadRequest("Unable to save changes, the budget was deleted !");
                }
                else
                {
                    return BadRequest("The budget you attempted to update was modified by another user after you got the original value, try again later !");
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Budgets
        [Authorize(Roles = "Admin, User")]
        public async Task<IHttpActionResult> PostBudget(Budget budget)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            context.Budgets.Add(budget);
            await context.SaveChangesAsync();

            return Created("PersoBankApi", budget);
        }

        // DELETE: api/Budgets/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IHttpActionResult> DeleteBudget(int id)
        {
            Budget budget = await context.Budgets.FindAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            context.Budgets.Remove(budget);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BudgetExists(id))
                {
                    return BadRequest("Unable to save changes, the budget has already deleted !");
                }
                else
                {
                    return BadRequest("The budget you attempted to delete was modified by another user after you got the original value, try again later !");
                }
            }
            return Ok(budget);
        }

        [Authorize(Roles = "Admin, User")]
        [Route("api/Budgets/GetAllUserBudgets")]
        [HttpPost]
        public IHttpActionResult GetAllUserBudgets([FromBody]string userId)
        {
            BudgetDTO budgetDTO;
            List<BudgetDTO> budgetDtoList = new List<BudgetDTO>();
            List<Transaction> budgetTransactionsList = new List<Transaction>();

            List<Budget> listBudget = _budgetDBAccess.FindByUser(userId);
            foreach (Budget budget in listBudget)
            {
                budgetTransactionsList = _transactionDBAccess.FindAllBudgetTransactions(budget);

                decimal expendedAmount = _businessService.GetTotalAmount(budgetTransactionsList);

                budgetDTO = new BudgetDTO
                {
                    BudgetId = budget.BudgetId,
                    BeginingDate = DateConverter.DateTimeToLong(budget.BeginingDate),
                    EndDate = DateConverter.DateTimeToLong(budget.EndDate),
                    Amount = budget.Amount,
                    ExpendedAmount = expendedAmount,
                    CategoryName = budget.Category.Label,
                    AccountName = budget.BankAccount.Name,
                    AccountId = budget.BankAccount.BankAccountId
                };
                budgetDtoList.Add(budgetDTO);
            }
            return Ok(budgetDtoList);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BudgetExists(int id)
        {
            return context.Budgets.Count(e => e.BudgetId == id) > 0;
        }
    }
}