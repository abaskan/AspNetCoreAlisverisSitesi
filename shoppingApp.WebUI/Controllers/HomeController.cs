using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using shoppingApp.Business.Abstract;
using shoppingApp.WebUI.Models;
using System.Net.Http;
using shoppingApp.Entity;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace shoppingApp.WebUI.Controllers
{
    public class HomeController:Controller
    {      
        private IProductService _productService;
        private ICategoryService _categoryService;
        public HomeController(IProductService productService,ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        
        public IActionResult Index()
        {
            var productViewModel = new ProductListViewModel()
            {
                Products = _productService.GetHomePageProducts()
            };

            return View(productViewModel);
        }
    }
}