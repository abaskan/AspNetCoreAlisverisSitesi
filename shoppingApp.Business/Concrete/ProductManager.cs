using System.Collections.Generic;
using shoppingApp.Business.Abstract;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.Entity;
using System.Threading.Tasks;

namespace shoppingApp.Business.Concrete
{
    public class ProductManager : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool Create(Product entity)
        {
           
            _unitOfWork.ProductRepository.Create(entity);
            return true;
            
        }

        public async Task<Product> CreateAsync(Product entity)
        {
            await _unitOfWork.ProductRepository.CreateAsync(entity);
            await _unitOfWork.SaveAsync();
            return entity;
        }

        public void Delete(Product entity)
        {
            _unitOfWork.ProductRepository.Delete(entity);
            _unitOfWork.Save();
        }

        //public async Task<IDataResult<List<Product>>> GetAll()
        public async Task<IDataResult<List<Product>>> GetAll()
        {
            return new SuccessDataResult<List<Product>>( await _unitOfWork.ProductRepository.GetAll());
        }

        public async Task<Product> GetById(int id)
        {
            return await _unitOfWork.ProductRepository.GetById(id);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _unitOfWork.ProductRepository.GetByIdWithCategories(id);
        }

        public int GetCountByCategory(string category)
        {
            return _unitOfWork.ProductRepository.GetCountByCategory(category);
        }

        public List<Product> GetHomePageProducts()
        {
            return _unitOfWork.ProductRepository.GetHomePageProducts();
        }

        public Product GetProductDetails(string url)
        {
            return _unitOfWork.ProductRepository.GetProductDetails(url);
        }

        public List<Product> GetProductsByCategory(string category, int page, int pageSize)
        {
            return _unitOfWork.ProductRepository.GetProductsByCategory(category,page,pageSize);
        }

        public List<Product> GetSearchResult(string searchString)
        {
            return _unitOfWork.ProductRepository.GetSearchResult(searchString);
        }

        public void Update(Product entity)
        {

            _unitOfWork.ProductRepository.Update(entity);
            _unitOfWork.Save();
        }

        public bool Update(Product entity, int[] categoryIds)
        {
            if(Validation(entity))
            {
                if (categoryIds.Length == 0)
                {
                    ErrorMessage += "Kategori seç";
                    return false;
                }
                _unitOfWork.ProductRepository.Update(entity,categoryIds);
                _unitOfWork.Save();
                return true;
            }
            return false;
        }

        public string ErrorMessage 
        {
            get ; 
            set ; 
        }

        public bool Validation(Product entity)
        {
            var isValid = true;

            if(string.IsNullOrEmpty(entity.Name))
            {
                ErrorMessage += "İsim alanı boş";
                isValid = false;
            }

            if(entity.Price < 0)
            {
                ErrorMessage += "Fiyat alanı sıfırdan küçük";
                isValid = false;
            }

            return isValid;

        }

        public bool Create(Product entity, int[] categoryIds)
        {
            _unitOfWork.ProductRepository.Create(entity,categoryIds);
            _unitOfWork.Save();
            return true;
        }

        public async Task UpdateAsync(Product entityToUpdate, Product entity)
        {
            entityToUpdate.Name = entity.Name;
            entityToUpdate.Price = entity.Price;
            entityToUpdate.Description = entity.Description;

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Product entity)
        {
            _unitOfWork.ProductRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}