using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using shoppingApp.DataAccess.Concrete.EFCore;
using shoppingApp.WebUI.Identity;

namespace shoppingApp.WebUI.Extensions
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>())
                {
                    try
                    {
                        identityContext.Database.Migrate();
                    }
                    catch (System.Exception)
                    {
                        // loglama
                        throw;
                    }
                }

                using (var shoppingContext = scope.ServiceProvider.GetRequiredService<ShoppingContext>())
                {
                    try
                    {
                        shoppingContext.Database.Migrate();
                    }
                    catch (System.Exception)
                    {
                        // loglama
                        throw;
                    }
                }
            }
       
            return host;
        }
    }
}