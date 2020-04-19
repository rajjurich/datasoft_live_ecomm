namespace Domain.Migrations
{
    using License;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<Domain.Core.EntitiesContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Domain.Core.EntitiesContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var x = context.States.Any();
            if (!x)
            {
                context.States.AddOrUpdate(s => s.StateId, new Domain.Entities.State { StateId = 1, StateName = "Maharashtra" });
                context.Districts.AddOrUpdate(d => d.DistrictId, new Domain.Entities.District { DistrictId = 1, StateId = 1, DistrictName = "Thane" });
                context.Categories.AddOrUpdate(c => c.CategoryId, new Domain.Entities.Category { CategoryId = 1, CategoryName = "General" });
                List<string> roleNames = new List<string>() { "superadmin", "admin", "user" };
                foreach (var roleName in roleNames)
                {
                    context.Roles.AddOrUpdate(r => r.RoleId, new Domain.Entities.Role { RoleId = 1, RoleName = roleName });
                }
            }

            var y = context.ApplicationConfigurations.Any();
            if (!y)
            {
                context.ApplicationConfigurations.AddOrUpdate(p => p.ApplicationConfigurationId, new Domain.Entities.ApplicationConfiguration { ApplicationConfigurationId = new Guid("10360D0E-6FB5-48A4-9FFA-3D1E1C1E00DD"), ApplicationOwnerName = "!@#m", ApplicationVersion = "1.1", SystemInformation = Check.systemInfo + "$h!p" + Check.systemInfo, ExpiryDate = Check.productExpiryDate });
            }
        }
    }
}
