using EfIntegrationTesting.Database;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DemoLibrary.Tests;

public class IntegrationTests : IClassFixture<SqlServerContainerFixture>, IAsyncLifetime
{
   private readonly SqlServerContainerFixture _sqlServerContainerFixture;

   private DemoDbContext? _ctx;
   private DemoDbContext Ctx => _ctx ?? throw new InvalidOperationException("Test is not initialized yet.");

   public IntegrationTests(SqlServerContainerFixture sqlServerContainerFixture)
   {
      _sqlServerContainerFixture = sqlServerContainerFixture;
   }

   public async Task InitializeAsync()
   {
      var options = new DbContextOptionsBuilder<DemoDbContext>()
                    .UseSqlServer(_sqlServerContainerFixture.ConnectionString)
                    .Options;

      var ctx = new DemoDbContext(options);

      try
      {
         await ctx.Database.EnsureCreatedAsync();
         _ctx = ctx;
      }
      catch (Exception)
      {
         await ctx.DisposeAsync();
         throw;
      }
   }

   [Fact]
   public async Task Create_product()
   {
      Ctx.Products.Add(new Product(new Guid("9103D544-8AA6-4A97-B07C-9D88421EAAE8"), "A product"));
      await Ctx.SaveChangesAsync();

      var products = await Ctx.Products.ToListAsync();

      products.Should().HaveCount(1);
   }

   [Fact]
   public async Task Create_another_product_to_verify_that_cleanup_works()
   {
      Ctx.Products.Add(new Product(new Guid("CECDE76E-E6C2-466D-81CC-995C169BA9F6"), "Another product"));
      await Ctx.SaveChangesAsync();

      var products = await Ctx.Products.ToListAsync();

      products.Should().HaveCount(1);
   }

   public async Task DisposeAsync()
   {
      if (_ctx is not null)
      {
         // Cleanup
         await _ctx.Products.ExecuteDeleteAsync();

         await _ctx.DisposeAsync();
      }
   }
}
