using System.Collections.Generic;
using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        Product GetProductDetails(string url);
        List<Product> GetProductsByCategory(string category, int page, int pageSize);
        int GetCountByCategory(string category);
        List<Product> GetHomePageProducts();
        List<Product> GetSearchResult(string searchString);
        Product GetByIdWithCategories(int id);
        void Update(Product entity, int[] categoryIds);
        void Create(Product entity, int[] categoryIds);
    }
}