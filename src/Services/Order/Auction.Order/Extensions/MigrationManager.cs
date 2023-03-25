using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.Infrastructure.Data;

namespace Auction.Order.Extensions
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            // Scope oluşturmamızın nedeni ilgili host üzerindeki servis katmanına ulaşıp, istenilem servis
            // bu şekilde de çağırılabilir.
            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var orderContext = scope.ServiceProvider.GetRequiredService<OrderContext>();

                    // Eğer in memory çalışmayacaksa migrate etme dedik. Çünkü InMemory'de migrate işleminde hata alınmaktadır.
                    if (orderContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                    {
                        // EF Core ile geliyor.
                        orderContext.Database.Migrate();

                    }
                    OrderContextSeed.SeedAsync(orderContext).Wait();

                }
                catch (Exception)
                {

                    throw;
                }
            }

            return host;
        }
    }
}
