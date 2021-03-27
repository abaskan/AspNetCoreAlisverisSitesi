using System.Collections.Generic;

namespace shoppingApp.Entity
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Brand { get; set; }     
        public string Name { get; set; }
        public string Color { get; set; }          
        public string Url { get; set; }     
        public double? Price { get; set; } 
        public string Description { get; set; }         
        public string ImageUrl { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAtHome { get; set; }
        public int? StockQuantity { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}