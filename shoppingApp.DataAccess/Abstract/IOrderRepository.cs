using System.Collections.Generic;
using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Abstract
{
    public interface IOrderRepository : IRepository<Order>
    {
        List<Order> GetOrders(string userId);
    }
}