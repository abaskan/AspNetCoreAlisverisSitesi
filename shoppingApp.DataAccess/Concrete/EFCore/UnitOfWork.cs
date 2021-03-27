using shoppingApp.DataAccess.Abstract;
using System.Threading.Tasks;

namespace shoppingApp.DataAccess.Concrete.EFCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShoppingContext _context;

        public UnitOfWork(ShoppingContext context)
        {
            _context = context;
        }

        private EFCoreProductRepository _productRepository;
        private EfCoreOrderRepository _orderRepository;
        private EFCoreCategoryRepository _categoryRepository;
        private EFCoreCartRepository _cartRepository;
        private EFCoreAddressRepository _addressRepository;

        public IProductRepository ProductRepository => _productRepository = _productRepository ?? new EFCoreProductRepository(_context);

        public IOrderRepository OrderRepository => _orderRepository = _orderRepository ?? new EfCoreOrderRepository(_context);

        public ICategoryRepository CategoryRepository => _categoryRepository = _categoryRepository ?? new EFCoreCategoryRepository(_context);

        public ICartRepository CartRepository => _cartRepository = _cartRepository ?? new EFCoreCartRepository(_context);

        public IAddressRepository AddressRepository => _addressRepository = _addressRepository ?? new EFCoreAddressRepository(_context);

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}