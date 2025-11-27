using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScrapyardApp.Models;

namespace ScrapyardApp.Data
{
    public class ScrapyardDbContext : IdentityDbContext<ApplicationUser>
    {
         public ScrapyardDbContext(DbContextOptions<ScrapyardDbContext> options) : base(options) 
        { }

            public DbSet<Category> Categories { get; set; }
            public DbSet<ScrapItem> ScrapItems { get; set; }
            public DbSet<Customer> Customers { get; set; }
            public DbSet<Purchase> Purchases { get; set; }
            public DbSet<Sale> Sales { get; set; }
       
    }
}
