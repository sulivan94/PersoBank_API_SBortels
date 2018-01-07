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

namespace API_PersoBank.Controllers
{
    public class PlacesController : ApiController
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private PlaceDBAccess _placeDBAccess = new PlaceDBAccess();
        private TransactionDBAccess _transactionDBAccess = new TransactionDBAccess();
        private BusinessService _businessService = new BusinessService();

        [Authorize(Roles = "Admin, User")]
        public List<Place> GetAllPlaces()
        {
            return _placeDBAccess.FindAll();
        }

        // GET: api/Places/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IHttpActionResult> GetPlace(int id)
        {
            Place place = await context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }

            return Ok(place);
        }

        // PUT: api/Places/5
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> PutPlace(int id, Place place)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (id != place.PlaceId)
            {
                return BadRequest();
            }

            context.Entry(place).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaceExists(id))
                {
                    return BadRequest("Unable to save changes, the place was deleted !");
                }
                else
                {
                    return BadRequest("The place you attempted to update was modified by another user after you got the original value, try again later !");
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Places
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> PostPlace(Place place)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            context.Places.Add(place);
            await context.SaveChangesAsync();

            return Created("PersoBankApi", place);
        }

        // DELETE: api/Places/5
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> DeletePlace(int id)
        {
            Place place = await context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }

            List<Transaction> transactions = _transactionDBAccess.FindByPlace(id);
            if(transactions.Count > 0)
            {
                await setNullOnCascade(transactions);
            }

            context.Places.Remove(place);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exc)
            {
                if (!PlaceExists(id))
                {
                    return BadRequest("This place has already deleted !");
                }
                else
                {
                    return BadRequest("The place you attempted to delete was modified by another user after you got the original value, try again later !");
                }
            }
            return Ok(place);
        }

        [Authorize(Roles = "Admin, User")]
        [Route("api/Places/BestPlace")]
        [HttpPost]
        public IHttpActionResult GetPlaceWithHighestNumberOfTransactions([FromBody] DateTime date)
        {
            BestPlace bestPlace = new BestPlace();

            var places = _placeDBAccess.FindAll();
            foreach (Place place in places)
            {
                var transactions = _transactionDBAccess.FindByPlaceAndDate(place.PlaceId, date);
                if (transactions.Count > bestPlace.NbTransactions)
                {
                    bestPlace.Name = place.Name;
                    bestPlace.NbTransactions = transactions.Count;
                }
            }
            return Ok(bestPlace);
        }

        [Authorize(Roles = "Admin, User")]
        [Route("api/Places/AverageVariation/{variationPourcentage:int}")]
        public List<Place> GetPlacesByAverageVariationPourcentage(int variationPourcentage)
        {
            List<Place> correctPlaces = new List<Place>();
            List<Transaction> transactions = null;
            TransactionsDetail detail = null;

            List<Place> places = _placeDBAccess.FindAll();
            foreach (Place place in places)
            {
                transactions = _transactionDBAccess.FindByPlace(place.PlaceId);
                detail = _businessService.CalculTransactionsDetail(transactions);

                bool isCorrectVariation = _businessService.IsCorrectVariationPlace(place, detail, variationPourcentage);
                if (isCorrectVariation)
                {
                    correctPlaces.Add(place);
                }
            }
            return correctPlaces;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlaceExists(int id)
        {
            return context.Places.Count(e => e.PlaceId == id) > 0;
        }

        private async Task setNullOnCascade(List<Transaction> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list.ElementAt(i).PlaceId = null;
                context.Entry(list.ElementAt(i)).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}