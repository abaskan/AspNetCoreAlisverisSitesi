using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using shoppingApp.Business.Abstract;
using shoppingApp.Business.Concrete;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.DataAccess.Concrete.EFCore;
using shoppingApp.WebUI.EmailServices;
using shoppingApp.WebUI.Identity;

namespace shoppingApp.WebUI
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<IdentityContext>(options=> options.UseSqlite(_configuration.GetConnectionString("SqliteConnection")));
            //services.AddDbContext<ShoppingContext>(options=> options.UseSqlite(_configuration.GetConnectionString("SqliteConnection")));

            services.AddDbContext<IdentityContext>(options=> options.UseSqlServer(_configuration.GetConnectionString("MsSqlConnection")));
            services.AddDbContext<ShoppingContext>(options=> options.UseSqlServer(_configuration.GetConnectionString("MsSqlConnection")));

            services.AddIdentity<User,IdentityRole>().AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options=> {
                // password
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;

                // Lockout                
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true;

                // options.User.AllowedUserNameCharacters = "";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            services.ConfigureApplicationCookie(options=> {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(60);
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name = ".shoppingApp.Security.Cookie",
                    SameSite = SameSiteMode.Strict
                };
            });
            
            //services.AddScoped<IProductRepository,EFCoreProductRepository>();
            services.AddScoped<IProductService,ProductManager>();

            //services.AddScoped<ICategoryRepository,EFCoreCategoryRepository>();
            services.AddScoped<ICategoryService,CategoryManager>();

            //services.AddScoped<ICartRepository,EFCoreCartRepository>();
            services.AddScoped<ICartService,CartManager>();

            //services.AddScoped<IOrderRepository,EfCoreOrderRepository>();
            services.AddScoped<IOrderService,OrderManager>();

            //services.AddScoped<IAddressRepository,EFCoreAddressRepository>();
            services.AddScoped<IAddressService,AddressManager>();

            services.AddScoped<IUnitOfWork,UnitOfWork>();

            services.AddScoped<IEmailSender,SmtpEmailSender>(i=> 
                new SmtpEmailSender(
                    _configuration["EmailSender:Host"],
                    _configuration.GetValue<int>("EmailSender:Port"),
                    _configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    _configuration["EmailSender:UserName"],
                    _configuration["EmailSender:Password"])
                );

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IConfiguration configuration,UserManager<User> userManager, RoleManager<IdentityRole> roleManager,ICartService cartService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(),"node_modules")),
                RequestPath = "/modules"
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "orders", 
                    pattern: "siparislerim",
                    defaults: new {controller="Cart",action="GetOrders"}
                ); 

                endpoints.MapControllerRoute(
                    name: "checkout", 
                    pattern: "odeme",
                    defaults: new {controller="Cart",action="Checkout"}
                ); 

                endpoints.MapControllerRoute(
                    name: "cart", 
                    pattern: "sepetim",
                    defaults: new {controller="Cart",action="Index"}
                ); 

                endpoints.MapControllerRoute(
                    name: "adminuseredit", 
                    pattern: "admin/kullanıcı/{id?}",
                    defaults: new {controller="Admin",action="UserEdit"}
                ); 

                 endpoints.MapControllerRoute(
                    name: "adminusers", 
                    pattern: "admin/kullanıcı/liste",
                    defaults: new {controller="Admin",action="UserList"}
                );

                endpoints.MapControllerRoute(
                    name: "adminroles", 
                    pattern: "admin/rol/liste",
                    defaults: new {controller="Admin",action="RoleList"}
                );

                endpoints.MapControllerRoute(
                    name: "adminrolecreate", 
                    pattern: "admin/rol/olustur",
                    defaults: new {controller="Admin",action="RoleCreate"}
                );      

                endpoints.MapControllerRoute(
                    name: "adminroleedit", 
                    pattern: "admin/rol/{id?}",
                    defaults: new {controller="Admin",action="RoleEdit"}
                );   
                
                endpoints.MapControllerRoute(
                    name: "adminproducts", 
                    pattern: "admin/urunler",
                    defaults: new {controller="Admin",action="ProductList"}
                );

                endpoints.MapControllerRoute(
                    name: "adminproductcreate", 
                    pattern: "admin/urunler/olustur",
                    defaults: new {controller="Admin",action="ProductCreate"}
                );

                endpoints.MapControllerRoute(
                    name: "adminproductedit", 
                    pattern: "admin/urunler/{id?}",
                    defaults: new {controller="Admin",action="ProductEdit"}
                );

                 endpoints.MapControllerRoute(
                    name: "admincategories", 
                    pattern: "admin/kategoriler",
                    defaults: new {controller="Admin",action="CategoryList"}
                );

                endpoints.MapControllerRoute(
                    name: "admincategorycreate", 
                    pattern: "admin/kategoriler/olustur",
                    defaults: new {controller="Admin",action="CategoryCreate"}
                );

                endpoints.MapControllerRoute(
                    name: "admincategoryedit", 
                    pattern: "admin/kategoriler/{id?}",
                    defaults: new {controller="Admin",action="CategoryEdit"}
                );
               

                // localhost/search    
                endpoints.MapControllerRoute(
                    name: "search", 
                    pattern: "ara",
                    defaults: new {controller="Shop",action="search"}
                );

                endpoints.MapControllerRoute(
                    name: "productdetails", 
                    pattern: "{url}",
                    defaults: new {controller="Shop",action="details"}
                );

                endpoints.MapControllerRoute(
                    name: "products", 
                    pattern: "urunler/{category?}",
                    defaults: new {controller="Shop",action="list"}
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern:"{controller=Home}/{action=Index}/{id?}"
                );
            });

            SeedIdentity.Seed(userManager,roleManager,cartService,configuration).Wait();
        }
    }
}
