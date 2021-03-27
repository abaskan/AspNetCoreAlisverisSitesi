using System.Collections.Generic;
using shoppingApp.Entity;

namespace shoppingApp.Business.Abstract
{
    public interface IOrderService
    {
        void Create(Order entity);
        List<Order> GetOrders(string userId);
    }
}