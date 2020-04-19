namespace API.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<API.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(API.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var x = context.Roles.Any();
            if (!x)
            {
                List<string> roleNames = new List<string>() { "superadmin", "admin", "user" };
                foreach (var roleName in roleNames)
                {
                    if (!context.Roles.Any(r => r.Name == roleName))
                    {
                        var store = new RoleStore<IdentityRole>(context);
                        var rolemanager = new RoleManager<IdentityRole>(store);
                        var role = new IdentityRole { Name = roleName };
                        rolemanager.Create(role);
                    }
                }


                List<RegisterBindingModel> registerBindingModels = new List<RegisterBindingModel>();
                registerBindingModels.Add(new RegisterBindingModel { Username = "superadmin", Email = "superadmin@example.com", Password = "SuperAdmin@123", Role = "superadmin" });
                registerBindingModels.Add(new RegisterBindingModel { Username = "admin", Email = "admin@example.com", Password = "Admin@123", Role = "superadmin" });
                registerBindingModels.Add(new RegisterBindingModel { Username = "dsadmin", Email = "dsadmin@example.com", Password = "Admin@123", Role = "admin" });

                foreach (var item in registerBindingModels)
                {
                    if (!context.Users.Any(u => u.UserName == item.Username))
                    {
                        var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
                        var user = new ApplicationUser { UserName = item.Username, Email = item.Email };
                        manager.Create(user, item.Password);
                        manager.AddToRole(user.Id, item.Role);
                    }
                }
            }
        }
    }
}
