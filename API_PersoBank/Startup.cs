using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Data.Entity;
using API_PersoBank.Models;

[assembly: OwinStartup(typeof(API_PersoBank.Startup))]

namespace API_PersoBank
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Database.SetInitializer(new PersoBankDBInitializer());
        }
    }
}
