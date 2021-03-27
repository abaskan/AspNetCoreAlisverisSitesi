using System.Linq;
using Microsoft.EntityFrameworkCore;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Concrete.EFCore
{
    public class EFCoreCartRepository : EfCoreGenericRepository<Cart>
                                        , ICartRepository

    {
        public EFCoreCartRepository(ShoppingContext context) : base(context)
        {
            
        }

        private ShoppingContext ShoppingContext
        {
            get { return context as ShoppingContext; }
        }

        public void ClearCart(int cartId)
        {
            
            var cmd = "delete from Cartitems where CartId=@p0";
            ShoppingContext.Database.ExecuteSqlRaw(cmd,cartId);
            
        }

        public void DeleteFromCart(int cartId, int productId)
        {
            
            var cmd = "delete from Cartitems where CartId=@p0 and ProductId=@p1";
            ShoppingContext.Database.ExecuteSqlRaw(cmd,cartId,productId);
            
        }

        public Cart GetByUserId(string userId)
        {
           
            return ShoppingContext.Carts
                    .Include(i=>i.CartItems)
                    .ThenInclude(i=>i.Product)
                    .FirstOrDefault(i=>i.UserId==userId);
        }

        public override void Update(Cart entity)
        {
            ShoppingContext.Carts.Update(entity);
        }
    }
}