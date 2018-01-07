using System;
using System.Web;
using System.Web.Http;
using API_PersoBank.Models;
using API_PersoBank.DBAccess;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace API_PersoBank.Controllers
{
    public class UsersController : ApiController
    {
        private UserDBAccess _userDBAccess = new UserDBAccess();
        private ApplicationUserManager _userManager;

        public UsersController() { }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        [Route("api/Users/GetByUserName")]
        [HttpPost]
        public IHttpActionResult GetByUserName([FromBody]string userName)
        {
            var user = _userDBAccess.FindByUserName(userName);
            if (user != null)
            {
                return Ok(new UserModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    EmailAddress = user.Email
                });
            }
            return NotFound();
        }

        [AllowAnonymous]
        [Route("api/Users/IsUserNameExists")]
        [HttpPost]
        public IHttpActionResult IsUserNameAlreadyExists([FromBody]string userName)
        {
            var user = _userDBAccess.FindByUserName(userName);
            if(user == null)
            {
                return NotFound();
            }
            return Ok();
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("api/Users/Register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applicationUser = new ApplicationUser()
            {
                LastName = user.LastName,
                FirstName = user.FirstName,
                UserName = user.UserName,
                Email = user.Email,
                Sex = user.Sex,
                BirthDate = user.BirthDate,
                InscriptionDate = DateTime.Now
            };

            IdentityResult result = await UserManager.CreateAsync(applicationUser, user.Password);

            if (!result.Succeeded)
            {
                return InternalServerError();
            }

            var newUser = UserManager.FindByName(user.UserName);
            UserManager.AddToRoles(newUser.Id, "User");

            return Created("PersoBankApi", user);
        }

        [Authorize(Roles = "Admin, User")]
        [Route("api/Users/GetLastRegistered")]
        [HttpPost]
        public IHttpActionResult GetLastRegistered([FromBody] DateTime date)
        {
            var users = _userDBAccess.FindLastRegistered(date);
            return Ok(users.Count);
        }
    }
}