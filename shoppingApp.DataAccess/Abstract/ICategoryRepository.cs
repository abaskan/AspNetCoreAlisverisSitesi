using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Category GetByIdWithProducts(int id);
        void DeleteFromCategory(int productId, int categoryId);
    }
}