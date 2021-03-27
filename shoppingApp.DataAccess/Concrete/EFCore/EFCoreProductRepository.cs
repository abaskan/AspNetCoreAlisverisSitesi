using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Concrete.EFCore
{
    public class EFCoreProductRepository :
    EfCoreGenericRepository<Product>, IProductRepository
    {
        public EFCoreProductRepository(ShoppingContext context) : base(context)
        {
            
        }

        private ShoppingContext ShoppingContext
        {
            get { return context as ShoppingContext; }
        }

        public void Create(Product entity, int[] categoryIds)
        {
           
            var product = ShoppingContext.Products
                .FirstOrDefault(i =>i.ProductId == entity.ProductId);

            if(product != null)
            {
                    product.ProductCategories = categoryIds.Select(catId => new ProductCategory()
                {
                    ProductId = entity.ProductId,
                    CategoryId = catId
                }).ToList();
            }

            ShoppingContext.Products.Add(entity);
        }

        public Product GetByIdWithCategories(int id)
        {
            
            return ShoppingContext.Products
                .Where(i => i.ProductId == id)
                .Include(i => i.ProductCategories)
                .ThenInclude(i => i.Category)
                .FirstOrDefault();
            
        }

        public int GetCountByCategory(string category)
        {
            
            var products = ShoppingContext.Products.Where(i => i.IsApproved).AsQueryable();

            if(!string.IsNullOrEmpty(category))
            {
                products = products
                    .Include(i => i.ProductCategories)
                    .ThenInclude(i => i.Category)
                    .Where(i => i.ProductCategories.Any(a => a.Category.Url.ToLower() == category.ToLower()));
            }
            return products.Count();
            
        }

        public List<Product> GetHomePageProducts()
        {
            
            return ShoppingContext.Products.Where(i => i.IsApproved && i.IsAtHome).ToList();
            
        }

        public Product GetProductDetails(string url)
        {
           
            return ShoppingContext.Products
                .Where(i => i.Url == url)
                .Include(i => i.ProductCategories)
                .ThenInclude(i => i.Category)
                .FirstOrDefault();
            
        }

        public List<Product> GetProductsByCategory(string category, int page, int pageSize)
        {
            
            var products = ShoppingContext.Products.Where(i => i.IsApproved).AsQueryable();

            if(!string.IsNullOrEmpty(category))
            {
                products = products
                    .Include(i => i.ProductCategories)
                    .ThenInclude(i => i.Category)
                    .Where(i => i.ProductCategories.Any(a => a.Category.Url.ToLower() == category.ToLower()));
            }
            return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<Product> GetSearchResult(string searchString)
        {
           
            var products = ShoppingContext.Products
                .Where(i => i.IsApproved && (i.Name.ToLower().Contains(searchString.ToLower()) || i.Description.ToLower().Contains(searchString.ToLower())))
                .AsQueryable();

            return products.ToList();
            //return context.Products.Where(i => i.IsApproved && (i.Name.ToLower().Contains(searchString.ToLower()) || i.Description.ToLower().Contains(searchString.ToLower()))).ToList();
            
        }

        public void Update(Product entity, int[] categoryIds)
        {
            
            var product = ShoppingContext.Products
                .Include(i => i.ProductCategories)
                .FirstOrDefault(i =>i.ProductId == entity.ProductId);

            
            if(product != null)
            {
                product.Name = entity.Name;
                product.Brand = entity.Brand;
                product.Color = entity.Color;
                product.Price = entity.Price;
                product.Description = entity.Description;
                product.Url = entity.Url;
                product.ImageUrl = entity.ImageUrl;
                product.IsApproved = entity.IsApproved;
                product.IsAtHome = entity.IsAtHome;
                product.ProductCategories = categoryIds.Select(catId => new ProductCategory()
                {
                    ProductId = entity.ProductId,
                    CategoryId = catId
                }).ToList();
            }
            
        }
    }
}