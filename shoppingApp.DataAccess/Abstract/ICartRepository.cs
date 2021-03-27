using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Abstract
{
    public interface ICartRepository : IRepository<Cart>
    {
        Cart GetByUserId(string userId);
        void DeleteFromCart(int cartId,int productId);
        void ClearCart(int cartId);
    }
}