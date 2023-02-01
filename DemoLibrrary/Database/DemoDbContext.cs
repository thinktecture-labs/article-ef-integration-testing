using Microsoft.EntityFrameworkCore;

namespace EfIntegrationTesting.Database;

public class DemoDbContext : DbContext
{
   public DbSet<Product> Products => Set<Product>();

   public DemoDbContext(DbContextOptions<DemoDbContext> options)
      : base(options)
   {
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<Product>(builder =>
                                   {
                                      builder.ToTable("Products");

                                      builder.Property(p => p.Name).HasMaxLength(200);
                                   });
   }
}
