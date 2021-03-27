using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using shoppingApp.Business.Abstract;
using System.Threading.Tasks;

namespace shoppingApp.WebUI.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private ICategoryService _categoryService;

        public CategoriesViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (RouteData.Values["category"] != null)
                 ViewBag.SelectedCategory = RouteData?.Values["category"];

            return View( await _categoryService.GetAll());
        }
    }
}