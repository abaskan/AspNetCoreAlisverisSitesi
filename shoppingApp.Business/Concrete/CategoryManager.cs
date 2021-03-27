using System.Collections.Generic;
using shoppingApp.Business.Abstract;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.Entity;
using System.Threading.Tasks;

namespace shoppingApp.Business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string ErrorMessage { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void Create(Category entity)
        {
            _unitOfWork.CategoryRepository.Create(entity);
            _unitOfWork.Save();
        }

        public async Task<Category> CreateAsync(Category entity)
        {
            await _unitOfWork.CategoryRepository.CreateAsync(entity);
            await _unitOfWork.SaveAsync();
            return entity;
        }

        public void Delete(Category entity)
        {
            _unitOfWork.CategoryRepository.Delete(entity);
            _unitOfWork.Save();
        }

        public void DeleteFromCategory(int productId, int categoryId)
        {
            _unitOfWork.CategoryRepository.DeleteFromCategory(productId,categoryId);
        }

        public async Task<List<Category>> GetAll()
        {
            return await _unitOfWork.CategoryRepository.GetAll();
        }

        public async Task<Category> GetById(int id)
        {
            return await _unitOfWork.CategoryRepository.GetById(id);
        }

        public Category GetByIdWithProducts(int id)
        {
            return _unitOfWork.CategoryRepository.GetByIdWithProducts(id);
        }

        public void Update(Category entity)
        {
            _unitOfWork.CategoryRepository.Update(entity);
            _unitOfWork.Save();
        }

        public bool Validation(Category entity)
        {
            throw new System.NotImplementedException();
        }
    }
}