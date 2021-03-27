using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shoppingApp.Business.Abstract;
using shoppingApp.Entity;
using shoppingApp.WebUI.EmailServices;
using shoppingApp.WebUI.Extensions;
using shoppingApp.WebUI.Identity;
using shoppingApp.WebUI.Models;

namespace shoppingApp.WebUI.Controllers
{
    public class AccountController:Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;
        private ICartService _cartService;
        private IAddressService _addressService;
        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager,IEmailSender emailSender, ICartService cartService, IAddressService addressService)
        {
            _userManager=userManager;
            _signInManager=signInManager;
            _emailSender =emailSender;
            _cartService = cartService;
            _addressService = addressService;
        }

        public async Task<IActionResult> AddressEdit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var entity = await _addressService.GetById((int)id);

            if(entity==null)
            {
                return NotFound();
            }

            var model = new AddressModel()
            {
                Id = entity.AddressId,
                AddressTitle = entity.AddressTitle,
                UserId = entity.UserId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Phone = entity.Phone,
                City = entity.City,
                District = entity.District,
                Neighborhood = entity.Neighborhood,
                AddressBody = entity.AddressBody
            };

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddressEdit(AddressModel model)
        {
            if(ModelState.IsValid)
            {
                var entity = await _addressService.GetById(model.Id);

                if(entity==null)
                {
                    return NotFound();
                }
                
                entity.AddressTitle = model.AddressTitle;
                entity.UserId = model.UserId;
                entity.FirstName = model.FirstName;
                entity.LastName = model.LastName;
                entity.Phone = model.Phone;
                entity.City = model.City;
                entity.District = model.District;
                entity.Neighborhood = model.Neighborhood;
                entity.AddressBody = model.AddressBody;

                _addressService.Update(entity);

                var msg = new AlertMessage()
                {            
                    Message = $"{entity.AddressTitle} isimli adres güncellendi.",
                    AlertType = "success"
                };

                TempData["message"] =  JsonConvert.SerializeObject(msg);

                return RedirectToAction("AddressList");
            }
            return View(model);
        }

        public IActionResult AddressList()
        {
            var userId = _userManager.GetUserId(User);
            return View(new AddressListViewModel()
            {
                Addresses = _addressService.GetByUserId(userId)
            });
        }


        public async Task<IActionResult> AddressDelete(int addressId)
        {
            var entity = await _addressService.GetById(addressId);
            if(entity!=null)
            {
                _addressService.Delete(entity);
            }

              var msg = new AlertMessage()
            {            
                Message = $"{entity.AddressTitle} isimli adres silindi.",
                AlertType = "danger"
            };

            TempData["message"] =  JsonConvert.SerializeObject(msg);

            return Redirect("AddressList");
        }

        public async Task<IActionResult> Manage()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            
            if(user!=null)
            {
                
                return View(new UserModel(){
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.PhoneNumber
                });
            }
            //return Redirect("Manage");
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Manage(UserModel model)
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
                    user.PhoneNumber = model.Phone;

                    var result = await _userManager.UpdateAsync(user);

                    if(result.Succeeded)
                    {
                        return Redirect("Login");
                    }
                }
                return Redirect("Manage");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if(user!=null)
                {
                    

                    var result = await _userManager.ChangePasswordAsync(user,model.Password,model.NewPassword);

                    if(result.Succeeded)
                    {
                        return Redirect("Manage");
                        
                    }
                }
                return Redirect("Manage");
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
                if(user!=null)
                {
                    

                    var result = await _userManager.DeleteAsync(user);

                    if(result.Succeeded)
                    {
                        await _signInManager.SignOutAsync();
                        TempData.Put("message", new AlertMessage()
                        {
                            Title="Hesabınız Silindi.",
                            Message="Hesabınız güvenli bir şekilde silindi.",
                            AlertType="danger"
                        });
                        return Redirect("~/");
                        
                    }
                    return Redirect("Manage");
                }
                
            return View();
        }


        public IActionResult AddressCreate()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult AddressCreate(AddressModel model)
        {
            var userId = _userManager.GetUserId(User);
            var entity = new Address()
            {
                AddressTitle = model.AddressTitle,
                UserId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,
                City = model.City,
                District = model.District,
                Neighborhood = model.Neighborhood,
                AddressBody = model.AddressBody

            };

            _addressService.Create(entity);
            
            return Redirect("AddressList");
        }



        public IActionResult Login(string ReturnUrl=null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(!ModelState.IsValid)
            {   
                return View(model);
            }

            // var user = await _userManager.FindByNameAsync(model.UserName);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user==null)
            {
                ModelState.AddModelError("","Bu kullanıcı adı ile daha önce hesap oluşturulmamış");
                return View(model);
            } 

            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("","Lütfen email hesabınıza gelen link ile üyeliğinizi onaylayınız.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user,model.Password,true,false);

            if(result.Succeeded) 
            {
                return Redirect(model.ReturnUrl??"~/");
            }

            ModelState.AddModelError("","Girilen kullanıcı adı veya parola yanlış");
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirstName  = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };           

            var result = await _userManager.CreateAsync(user,model.Password);
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user,"Customer");
                // generate token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail","Account",new {
                    userId = user.Id,
                    token = code
                });

                // email
                await _emailSender.SendEmailAsync(model.Email,"Hesabınızı onaylayınız.",$"Lütfen email hesabınızı onaylamak için linke <a href='https://localhost:5001{url}'>tıklayınız.</a>");
                return RedirectToAction("Login","Account");
            }           

            ModelState.AddModelError("","Bilinmeyen hata oldu lütfen tekrar deneyiniz.");
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData.Put("message", new AlertMessage()
            {
                Title="Oturum Kapatıldı.",
                Message="Hesabınız güvenli bir şekilde kapatıldı.",
                AlertType="warning"
            });
            return Redirect("~/");
        }

        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if(userId==null || token ==null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title="Geçersiz token.",
                    Message="Geçersiz Token",
                    AlertType="danger"
                });
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if(user!=null)
            {
                var result = await _userManager.ConfirmEmailAsync(user,token);
                if(result.Succeeded)
                {
                    // cart oluşturma
                    _cartService.InitializeCart(user.Id);

                    TempData.Put("message", new AlertMessage()
                    {
                        Title="Hesabınız onaylandı.",
                        Message="Hesabınız onaylandı.",
                        AlertType="success"
                    });
                    return View();
                }
            }
            TempData.Put("message", new AlertMessage()
            {
                Title="Hesabınızı onaylanmadı.",
                Message="Hesabınızı onaylanmadı.",
                AlertType="warning"
            });
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if(string.IsNullOrEmpty(Email))
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(Email);

            if(user==null)
            {
                return View();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("ResetPassword","Account",new {
                userId = user.Id,
                token = code
            });

            // email
            await _emailSender.SendEmailAsync(Email,"Reset Password",$"Parolanızı yenilemek için linke <a href='https://localhost:5001{url}'>tıklayınız.</a>");

            return View();
        }

        public IActionResult ResetPassword(string userId,string token)
        {
            if(userId==null || token==null)
            {
                return RedirectToAction("Home","Index");
            }

            var model = new ResetPasswordModel {Token=token};

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user==null)
            {
                return RedirectToAction("Home","Index");
            }

            var result = await _userManager.ResetPasswordAsync(user,model.Token,model.Password);

            if(result.Succeeded)
            {
                return RedirectToAction("Login","Account");
            }

            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}