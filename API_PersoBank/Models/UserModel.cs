using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_PersoBank.Models
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
    }

    public class User
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Sex { get; set; }
    }

    public class UserEmailModel
    {
        public string EmailAddress { get; set; }
    }

    public class UserNameModel
    {
        public string UserName { get; set; }
    }
}