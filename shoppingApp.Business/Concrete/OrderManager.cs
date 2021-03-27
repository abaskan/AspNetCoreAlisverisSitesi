using System.Collections.Generic;
using shoppingApp.Business.Abstract;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.Entity;

namespace shoppingApp.Business.Concrete
{
    public class OrderManager : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(Order entity)
        {
            _unitOfWork.OrderRepository.Create(entity);
            _unitOfWork.Save();
        }

        public List<Order> GetOrders(string userId)
        {
            return _unitOfWork.OrderRepository.GetOrders(userId);
        }
    }
}