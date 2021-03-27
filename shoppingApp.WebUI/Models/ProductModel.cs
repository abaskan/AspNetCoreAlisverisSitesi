using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using shoppingApp.Entity;

namespace shoppingApp.WebUI.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }  
       
        // [Display(Name="Name",Prompt="Enter product name")]
        // [Required(ErrorMessage="Name zorunlu bir alan.")]
        // [StringLength(60,MinimumLength=5,ErrorMessage="Ürün ismi 5-10 karakter aralığında olmalıdır.")]
        public string Brand { get; set; }     
        public string Name { get; set; }
        public string Color { get; set; }       
       
        //[Required(ErrorMessage="Url zorunlu bir alan.")]  
        public string Url { get; set; }     
      
        [Required(ErrorMessage="Price zorunlu bir alan.")]  
        [Range(1,10000,ErrorMessage="Price için 1-10000 arasında değer girmelisiniz.")]
        [RegularExpression(@"^\$?\d+(\.(\d{2}))?$")]
        public double? Price { get; set; } 
      
        [Required(ErrorMessage="Description zorunlu bir alan.")]
        [StringLength(9999999,MinimumLength=5,ErrorMessage="Description 5-9999999 karakter aralığında olmalıdır.")]

        public string Description { get; set; }      
       
        //[Required(ErrorMessage="ImageUrl zorunlu bir alan.")]  
        public string ImageUrl { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAtHome { get; set; }
        public int? StockQuantity { get; set; }
        public List<Category> SelectedCategories { get; set; }
    }
}