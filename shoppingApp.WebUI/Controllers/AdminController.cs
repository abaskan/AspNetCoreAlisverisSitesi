using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shoppingApp.Business.Abstract;
using shoppingApp.Entity;
using shoppingApp.WebUI.Extensions;
using shoppingApp.WebUI.Identity;
using shoppingApp.WebUI.Models;

namespace shoppingApp.WebUI.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController: Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        public AdminController(IProductService productService,
                               ICategoryService categoryService,
                               RoleManager<IdentityRole> roleManager,
                               UserManager<User> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user!=null)
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(i=>i.Name);

                ViewBag.Roles = roles;
                return View(new UserDetailsModel(){
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles
                });
            }
            return Redirect("~/admin/user/list");
        }


        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailsModel model,string[] selectedRoles)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if(user!=null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.EmailConfirmed = model.EmailConfirmed;

                    var result = await _userManager.UpdateAsync(user);

                    if(result.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        selectedRoles = selectedRoles?? new string[]{};
                        await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles).ToArray<string>());
                        await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles).ToArray<string>());

                        return Redirect("/admin/user/list");
                    }
                }
                return Redirect("/admin/user/list");
            }

            return View(model);

        }
        
        public IActionResult UserList()
        {
            return View(_userManager.Users);
        }

        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonmembers = new List<User>();

            foreach (var user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user,role.Name)
                                ?members:nonmembers;
                list.Add(user);
            }
            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            if(ModelState.IsValid)
            {
                foreach (var userId in model.IdsToAdd ?? new string[]{})
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if(user!=null)
                    {
                        var result = await _userManager.AddToRoleAsync(user,model.RoleName);
                        if(!result.Succeeded)
                        {
                              foreach (var error in result.Errors)
                              { 
                                ModelState.AddModelError("", error.Description);  
                              }  
                        }
                    }
                }
          
                foreach (var userId in model.IdsToDelete ?? new string[]{})
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if(user!=null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user,model.RoleName);
                        if(!result.Succeeded)
                        {
                              foreach (var error in result.Errors)
                              { 
                                ModelState.AddModelError("", error.Description);  
                              }  
                        }
                    }
                }
            }
            return Redirect("/admin/role/"+model.RoleId);
        }
        
        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]        
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if(result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                }
            }
            return View(model);
        }
        
        
        public async Task<IActionResult> ProductList()
        {
            var products = await _productService.GetAll();
            return View(new ProductListViewModel()
            {
                Products = products
            });
        }
        public async Task<IActionResult> CategoryList()
        {
            var categories = await _categoryService.GetAll();
            return View(new CategoryListViewModel()
            {
                Categories = categories
            });
        }
        public IActionResult ProductCreate()
        {
            ViewBag.Categories = _categoryService.GetAll();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductModel model,int[] categoryIds, IFormFile file)
        {
            if(ModelState.IsValid)
            {
                string imageUrl = await UploadFile(file);
                var entity = new Product()
                {
                    Brand = model.Brand,
                    Name = model.Name,
                    Color = model.Color,
                    Url = NameToUrl(model.Name) + "-" + NameToUrl(model.Color) + new Random().Next(111,999).ToString(),
                    Price = model.Price,
                    Description = model.Description,
                    ImageUrl = imageUrl,
                    StockQuantity = model.StockQuantity,
                    IsApproved = model.IsApproved,
                    IsAtHome = model.IsAtHome,
                    ProductCategories = categoryIds.Select(categoryId => new ProductCategory(){
                        ProductId = model.ProductId,
                        CategoryId = categoryId
                    }).ToList()
                };
                
                if(_productService.Create(entity))
                {                    
                    TempData.Put("message", new AlertMessage()
                    {
                        Title="kayıt eklendi",
                        Message="kayıt eklendi",
                        AlertType="success"
                    });
                    return RedirectToAction("ProductList");
                }
                TempData.Put("message", new AlertMessage()
                {
                    Title="hata",
                    Message=_productService.ErrorMessage,
                    AlertType="danger"
                });                

                return View(model);
            }            
            return View(model);         
        }
        public IActionResult CategoryCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CategoryCreate(CategoryModel model)
        {
             if(ModelState.IsValid)
            {
                var entity = new Category()
                {
                    Name = model.Name,
                    Url = NameToUrl(model.Name)         
                };
                
                _categoryService.Create(entity);

                TempData.Put("message", new AlertMessage()
                {
                    Title="kayıt eklendi.",
                    Message=$"{entity.Name} isimli category eklendi.",
                    AlertType="success"
                });             

                return RedirectToAction("CategoryList");
            }
            return View(model);
        }
       

        public IActionResult ProductEdit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var entity = _productService.GetByIdWithCategories((int)id);

            if(entity==null)
            {
                 return NotFound();
            }

            var model = new ProductModel()
            {
                ProductId = entity.ProductId,
                Brand = entity.Brand,
                Color = entity.Color,
                Name = entity.Name,
                Url = entity.Url,
                Price = entity.Price,
                ImageUrl= entity.ImageUrl,
                Description = entity.Description,
                StockQuantity = entity.StockQuantity,
                IsApproved = entity.IsApproved,
                IsAtHome = entity.IsAtHome,
                SelectedCategories = entity.ProductCategories.Select(i=>i.Category).ToList()
            };

            ViewBag.Categories = _categoryService.GetAll();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductModel model,int[] categoryIds,IFormFile file)
        {
            if(ModelState.IsValid)
            {        
                var entity = await _productService.GetById(model.ProductId);
                if(entity==null)
                {
                    return NotFound();
                }

                
                entity.Name = model.Name;
                entity.Brand = model.Brand;
                entity.Color = model.Color;

                entity.Price = model.Price;
                entity.Url = NameToUrl(model.Name) + "-" + NameToUrl(model.Color) + "-" + model.ProductId;
                entity.Description = model.Description;
                entity.IsAtHome = model.IsAtHome;
                entity.IsApproved = model.IsApproved;
                entity.StockQuantity = model.StockQuantity;

                if(file!=null)
                {
                    var extention = Path.GetExtension(file.FileName);
                    var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                    entity.ImageUrl = randomName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images",randomName);

                    using(var stream = new FileStream(path,FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                if(_productService.Update(entity,categoryIds))
                {                    
                     TempData.Put("message", new AlertMessage()
                    {
                        Title="kayıt güncellendi",
                        Message="kayıt güncellendi",
                        AlertType="success"
                    });  
                    return RedirectToAction("ProductList");
                }
                 TempData.Put("message", new AlertMessage()
                    {
                        Title="hata",
                        Message=_productService.ErrorMessage,
                        AlertType="danger"
                    }); 
            }
            ViewBag.Categories = await _categoryService.GetAll();
            return View(model);
        }



        public IActionResult CategoryEdit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var entity = _categoryService.GetByIdWithProducts((int)id);

            if(entity==null)
            {
                 return NotFound();
            }

            var model = new CategoryModel()
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Url = entity.Url,
                Products = entity.ProductCategories.Select(p=>p.Product).ToList()
            };
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> CategoryEdit(CategoryModel model)
        {
            if(ModelState.IsValid)
            {
                var entity = await _categoryService.GetById(model.CategoryId);
                if(entity==null)
                {
                    return NotFound();
                }
                entity.Name = model.Name;
                entity.Url = NameToUrl(model.Name);

                _categoryService.Update(entity);

                var msg = new AlertMessage()
                {            
                    Message = $"{entity.Name} isimli category güncellendi.",
                    AlertType = "success"
                };

                TempData["message"] =  JsonConvert.SerializeObject(msg);

                return RedirectToAction("CategoryList");
            }
            return View(model);
        }


        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var entity = await _productService.GetById(productId);

            if(entity!=null)
            {
                _productService.Delete(entity);
            }

              var msg = new AlertMessage()
            {            
                Message = $"{entity.Name} isimli ürün silindi.",
                AlertType = "danger"
            };

            TempData["message"] =  JsonConvert.SerializeObject(msg);

            return RedirectToAction("ProductList");
        }
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var entity = await _categoryService.GetById(categoryId);

            if(entity!=null)
            {
                _categoryService.Delete(entity);
            }

              var msg = new AlertMessage()
            {            
                Message = $"{entity.Name} isimli category silindi.",
                AlertType = "danger"
            };

            TempData["message"] =  JsonConvert.SerializeObject(msg);

            return RedirectToAction("CategoryList");
        }
    
        [HttpPost]
        public IActionResult DeleteFromCategory(int productId,int categoryId)
        {
            _categoryService.DeleteFromCategory(productId,categoryId);
            return Redirect("/admin/categories/"+categoryId);
        }

        public async Task<string> UploadFile (IFormFile file)
        {   string imageUrl = null;
            if(file!=null)
            {
                //var extention = Path.GetExtension(file.FileName);
                //var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images",fileName);
                imageUrl = fileName;
                using(var stream = new FileStream(path,FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return imageUrl;
        }

        public string NameToUrl(string name)
        {
            string url = name.ToLower()
                            .Replace("ç", "c")
                            .Replace("ğ", "g")
                            .Replace("ı", "i")
                            .Replace("ö", "o")
                            .Replace("ş", "s")
                            .Replace("ü", "u")
                            .Replace(" ", "-");

            return url;
        }
    }
}