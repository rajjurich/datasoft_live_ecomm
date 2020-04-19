using Domain.Entities;
using Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class EntitiesContext : DbContext
    {
        private ILog _ilog;
        public EntitiesContext()
            : base("Name=DefaultConnection")
        {
            _ilog = Log.GetInstance;
            this.Configuration.LazyLoadingEnabled = false;
            var check = ConfigurationManager.AppSettings["DBAuditLog"];
            var isEnabled = check == "true" ? true : false;
            if (isEnabled)
            {
                this.Database.Log = s => _ilog.Logger(s, Enumerations.LogType.Audit);
            }
        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ApplicationConfiguration> ApplicationConfigurations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductsPurchaseOrder> ProductsPurchaseOrders { get; set; }
        public DbSet<ProductsSalesOrder> ProductsSalesOrders { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<MenuAccess> MenuAccesses { get; set; }    

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Types().Configure(t => t.MapToStoredProcedures());
            //modelBuilder.Entity<Category>().MapToStoredProcedures();
            //modelBuilder.Entity<Company>().MapToStoredProcedures();
            //modelBuilder.Entity<Customer>().MapToStoredProcedures();
            //modelBuilder.Entity<Manufacturer>().MapToStoredProcedures();
            //modelBuilder.Entity<ProductType>().MapToStoredProcedures();
            //modelBuilder.Entity<Product>().MapToStoredProcedures();
            //modelBuilder.Entity<Resource>().MapToStoredProcedures();
            //modelBuilder.Entity<Vendor>().MapToStoredProcedures();
            //modelBuilder.Entity<Menu>().MapToStoredProcedures();
            //modelBuilder.Entity<Role>().MapToStoredProcedures();
            //modelBuilder.Entity<MenuAccess>().MapToStoredProcedures();           
            //base.OnModelCreating(modelBuilder);
        }
    }
}
