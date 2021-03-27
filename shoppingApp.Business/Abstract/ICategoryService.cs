using System.Collections.Generic;
using shoppingApp.Entity;
using System.Threading.Tasks;

namespace shoppingApp.Business.Abstract
{
    public interface ICategoryService
    {
        Task<Category> GetById(int id);
        Task<List<Category>> GetAll();
        void Create(Category entity);
        Task<Category> CreateAsync(Category entity);
        void Update(Category entity);
        void Delete(Category entity);
        Category GetByIdWithProducts(int id);
        void DeleteFromCategory(int productId, int categoryId);
    }
}