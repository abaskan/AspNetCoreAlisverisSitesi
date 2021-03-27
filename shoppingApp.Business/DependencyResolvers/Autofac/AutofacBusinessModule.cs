using Autofac;
using shoppingApp.Business.Abstract;
using shoppingApp.Business.Concrete;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.DataAccess.Concrete.EFCore;

namespace shoppingApp.Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProductManager>().As<IProductService>();
            builder.RegisterType<OrderManager>().As<IOrderService>();
            builder.RegisterType<CategoryManager>().As<ICategoryService>();
            builder.RegisterType<CartManager>().As<ICartService>();
            builder.RegisterType<AddressManager>().As<IAddressService>();
            builder.RegisterType<ProductManager>().As<IProductService>();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
        }
    }
}