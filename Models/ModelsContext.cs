using Microsoft.EntityFrameworkCore;

namespace SecurityLite.Models
{
    public class ModelsContext : DbContext
    {
        public ModelsContext(DbContextOptions<ModelsContext> options) : base(options)
        {

        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<GuardedObject> GuardedObjects { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
       
    }
}
