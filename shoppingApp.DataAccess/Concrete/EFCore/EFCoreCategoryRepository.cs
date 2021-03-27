using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Concrete.EFCore
{
    public class EFCoreCategoryRepository :
    EfCoreGenericRepository<Category>, ICategoryRepository
    {
        public EFCoreCategoryRepository(ShoppingContext context) : base(context)
        {
            
        }

        private ShoppingContext ShoppingContext
        {
            get { return context as ShoppingContext; }
        }

        public void DeleteFromCategory(int productId, int categoryId)
        {
            var command = "delete from productcategory where ProductId=@p0 and CategoryId=@p1";
            ShoppingContext.Database.ExecuteSqlRaw(command,productId,categoryId);
        }

        public Category GetByIdWithProducts(int id)
        {
            return ShoppingContext.Categories
                .Where(i => i.CategoryId == id)
                .Include(i => i.ProductCategories)
                .ThenInclude(i => i.Product)
                .FirstOrDefault();   
        }
    }
}