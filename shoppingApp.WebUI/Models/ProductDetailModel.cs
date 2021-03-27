using System.Collections.Generic;
using shoppingApp.Entity;

namespace shoppingApp.WebUI.Models
{
    public class ProductDetailModel
    {
        public Product Product { get; set; }
        public List<Category> Categories { get; set; }
    }
}