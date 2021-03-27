using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Concrete.EFCore
{
    public class EfCoreOrderRepository : EfCoreGenericRepository<Order>, IOrderRepository
    {
        public EfCoreOrderRepository(ShoppingContext context) : base(context)
        {
            
        }

        private ShoppingContext ShoppingContext
        {
            get { return context as ShoppingContext; }
        }


        public List<Order> GetOrders(string userId)
        {
            var orders = ShoppingContext.Orders
                            .Include(i => i.OrderItems)
                            .ThenInclude(i => i.Product)
                            .AsQueryable();
            
            if(!string.IsNullOrEmpty(userId))
            {
                orders = orders.Where(i => i.UserId == userId);
            }

            return orders.ToList();
        }
    }
}