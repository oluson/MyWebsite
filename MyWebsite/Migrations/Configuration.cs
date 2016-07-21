namespace MyWebsite.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MyWebsite.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MyWebsite.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin")) { roleManager.Create(new IdentityRole { Name = "Admin" }); }
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "oosajere@gmail.com"))
            { userManager.Create(new ApplicationUser {
                UserName = "oosajere@gmail.com",
                Email = "oosajere@gmail.com",
                FirstName = "Olu",
                LastName = "Osajere",
                DisplayName = "oluson" }, 
                "Wilson#123"); }

            var userId = userManager.FindByEmail("oosajere@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");
        }
    }
}
