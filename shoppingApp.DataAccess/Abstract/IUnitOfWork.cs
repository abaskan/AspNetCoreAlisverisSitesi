using System;
using System.Threading.Tasks;

namespace shoppingApp.DataAccess.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ICartRepository CartRepository { get; }
        IAddressRepository AddressRepository { get; }

        void Save();
        Task<int> SaveAsync();
    }
}