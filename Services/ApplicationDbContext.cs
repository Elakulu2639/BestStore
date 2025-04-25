using Microsoft.EntityFrameworkCore;
using BestStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BestStore.Services
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {

        
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
