using System.Collections.Generic;
using shoppingApp.Entity;
using System.Threading.Tasks;

namespace shoppingApp.Business.Abstract
{
    public interface IProductService 
    {
        Task<Product> GetById(int id);
        Task<IDataResult<List<Product>>> GetAll();
        bool Create(Product entity);
        Task<Product> CreateAsync(Product entity);
        void Update(Product entity);
        void Delete(Product entity);
        Task DeleteAsync(Product entity);        
        Product GetProductDetails(string url);
        List<Product> GetProductsByCategory(string category, int page, int pageSize);
        int GetCountByCategory(string category);
        List<Product> GetHomePageProducts();
        List<Product> GetSearchResult(string searchString);
        Product GetByIdWithCategories(int id);
        bool Update(Product entity, int[] categoryIds);
        Task UpdateAsync(Product entityToUpdate,Product entity);
        bool Create(Product entity, int[] categoryIds);
    }
}