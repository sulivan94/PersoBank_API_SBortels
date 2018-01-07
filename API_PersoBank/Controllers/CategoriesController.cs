using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using API_PersoBank.Models;
using API_PersoBank.DBAccess;
using API_PersoBank.Business;

namespace API_PersoBank.Controllers
{
    public class CategoriesController : ApiController
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private CategoryDBAccess _categoryDBAccess = new CategoryDBAccess();
        private TransactionDBAccess _transactionDBAccess = new TransactionDBAccess();
        private BusinessService _businessService = new BusinessService();

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public List<Category> GetCategories()
        {
            return _categoryDBAccess.FindAll();
        }

        // GET: api/Categories/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IHttpActionResult> GetCategory(int id)
        {
            Category category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/Categories/5
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> PutCategory(int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            context.Entry(category).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!CategoryExists(id))
                {
                    return BadRequest("Unable to save changes, the category was deleted !");
                }
                else
                {
                    return BadRequest("The category you attempted to update was modified by another user after you got the original value, try again later !");
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Categories
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> PostCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            context.Categories.Add(category);
            await context.SaveChangesAsync();

            return Created("PersoBankApi", category);
        }

        [Authorize(Roles = "Admin, User")]
        [Route("api/Categories/BestCategory")]
        [HttpPost]
        public IHttpActionResult GetCategoryWithHighestNumberOfTransactions([FromBody] DateTime date)
        {
            BestCategory bestCategory = new BestCategory();

            var categories = _categoryDBAccess.FindAll();
            foreach (Category category in categories)
            {
                var transactions = _transactionDBAccess.FindByCategoryAndDate(category.CategoryId, date);
                if (transactions.Count > bestCategory.NbTransactions)
                {
                    bestCategory.Label = category.Label;
                    bestCategory.NbTransactions = transactions.Count;
                }
            }
            return Ok(bestCategory);
        }

        [Authorize(Roles = "Admin, User")]
        [Route("api/Categories/AverageVariation/{variationPourcentage:int}")]
        public IHttpActionResult GetCategoriesByAverageVariationPourcentage(int variationPourcentage)
        {
            List<Category> correctCategories = new List<Category>();
            List<Transaction> transactions = null;
            TransactionsDetail detail = null;

            List<Category> categories = _categoryDBAccess.FindAll();
            foreach (Category category in categories)
            {
                transactions = _transactionDBAccess.FindByCategory(category.CategoryId);
                detail = _businessService.CalculTransactionsDetail(transactions);

                bool isCorrectVariation = _businessService.IsCorrectVariationCategory(category, detail, variationPourcentage);
                if (isCorrectVariation)
                {
                    correctCategories.Add(category);
                }
            }
            return Ok(correctCategories);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoryExists(int id)
        {
            return context.Categories.Count(e => e.CategoryId == id) > 0;
        }
    }
}