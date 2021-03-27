using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shoppingApp.Business.Abstract;
using shoppingApp.Entity;
using shoppingApp.WebUI.Identity;
using shoppingApp.WebUI.Models;

namespace shoppingApp.WebUI.Controllers
{
    [Authorize]
    public class CartController:Controller
    {
        private ICartService _cartService;
        private IOrderService _orderService;
        private IAddressService _adresService;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        public CartController(IOrderService orderService,ICartService cartService,IAddressService adresService,UserManager<User> userManager,SignInManager<User> signInManager)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userManager = userManager;
            _adresService = adresService;
            _signInManager = signInManager;
        }


        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));
            return View(new CartModel(){
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i=>new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    Name = i.Product.Name,
                    Price = (double)i.Product.Price,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity =i.Quantity

                }).ToList()
            });
        } 

        [HttpPost]
        public IActionResult AddToCart(int productId,int quantity)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.AddToCart(userId,productId,quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId,int quantity)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.UpdateQuantity(userId,productId,quantity);
            return RedirectToAction("Index");
        }

        

        [HttpPost]
        public IActionResult DeleteFromCart(int productId)
        {
             var userId = _userManager.GetUserId(User);
            _cartService.DeleteFromCart(userId,productId);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));
            
            var orderModel = new OrderModel();

            orderModel.CartModel = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i=>new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    Name = i.Product.Name,
                    Price = (double)i.Product.Price,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity =i.Quantity

                }).ToList()
            };

            //var address = _adresService.GetByUserId(_userManager.GetUserId(User));


            ViewBag.AllAddress = _adresService.GetByUserId(_userManager.GetUserId(User));
            
            return View(orderModel);
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutAsync(OrderModel model,int address)
        {
            ViewBag.AllAddress = _adresService.GetByUserId(_userManager.GetUserId(User));
            if(ModelState.IsValid)
            {     
                var userId = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(userId);  
                var selectedAddress = await _adresService.GetById(address);
                //Console.WriteLine(selectedAddress.AddressBody)  ;  
                var cart = _cartService.GetCartByUserId(userId);

                model.CartModel = new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(i=>new CartItemModel()
                    {
                        CartItemId = i.Id,
                        ProductId = i.ProductId,
                        Name = i.Product.Name,
                        Price = (double)i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        Quantity =i.Quantity
                    }).ToList()
                };

                model.selectedAddress = new AddressModel()
                {
                    Id = selectedAddress.AddressId,
                    AddressTitle = selectedAddress.AddressTitle,
                    UserId = userId,
                    FirstName = selectedAddress.FirstName,
                    LastName = selectedAddress.LastName,
                    Phone = selectedAddress.Phone,
                    City = selectedAddress.City,
                    District = selectedAddress.District,
                    Neighborhood = selectedAddress.Neighborhood,
                    AddressBody = selectedAddress.AddressBody
                };

                //Console.WriteLine(model.selectedAddress.FirstName);

                //Console.WriteLine(address);

                var payment = PaymentProcess(model);

                if(payment.Status=="success")
                {
                    SaveOrder(model,payment,user);
                    ClearCart(model.CartModel.CartId);
                    return View("Success");
                }else
                {
                    var msg = new AlertMessage()
                    {            
                        Message = $"{payment.ErrorMessage}",
                        AlertType = "danger"
                    };

                    TempData["message"] =  JsonConvert.SerializeObject(msg);
                }
            }
            return View(model);
        }

        private void ClearCart(int cartId)
        {
            _cartService.ClearCart(cartId);
        }

        private void SaveOrder(OrderModel model, Payment payment, User user)
        {
           var order = new Order();

           order.OrderNumber = new Random().Next(111111,999999).ToString();
           order.OrderState = EnumOrderState.completed;
           order.PaymentType = EnumPaymentType.CreditCard;
           order.PaymentId = payment.PaymentId;
           order.ConversationId = payment.ConversationId;
           order.OrderDate = DateTime.Parse(DateTime.Now.ToString("F", new CultureInfo("tr-TR")));
           order.AddressTitle =model.selectedAddress.AddressTitle;
           order.FirstName = model.selectedAddress.FirstName;
           order.LastName = model.selectedAddress.LastName;
           order.Phone = model.selectedAddress.Phone;
           order.City = model.selectedAddress.City;
           order.District = model.selectedAddress.District;
           order.Neighborhood = model.selectedAddress.Neighborhood;
           order.AddressBody = model.selectedAddress.AddressBody;

           

            order.Email = user.Email;

           // var userEmail = await _userManager.GetEmailAsync(_userManager.FindByIdAsync(userId));

           order.UserId = user.Id;


           order.OrderItems = new List<Entity.OrderItem>();

            foreach (var item in model.CartModel.CartItems)
            {
                var orderItem = new shoppingApp.Entity.OrderItem()
                {
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId
                };
                order.OrderItems.Add(orderItem);
            }
            _orderService.Create(order);
        }

        private Payment PaymentProcess(OrderModel model)
        {
            Options options = new Options();
            options.ApiKey = "api-key";
            options.SecretKey = "secret-key";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";
                    
            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(111111111,999999999).ToString();
            request.Price = model.CartModel.TotalPrice().ToString();
            request.PaidPrice = model.CartModel.TotalPrice().ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = model.Id.ToString();
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;
            paymentCard.ExpireYear = model.ExpirationYear;
            paymentCard.Cvc = model.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            //  paymentCard.CardNumber = "5528790000000008";
            // paymentCard.ExpireMonth = "12";
            // paymentCard.ExpireYear = "2030";
            // paymentCard.Cvc = "123";

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = model.selectedAddress.FirstName;
            buyer.Surname = model.selectedAddress.LastName;
            buyer.GsmNumber = model.selectedAddress.Phone;
            buyer.Email = model.Email;
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = model.selectedAddress.Neighborhood + " " + model.selectedAddress.AddressBody + model.selectedAddress.District;
            buyer.Ip = "85.34.78.112";
            buyer.City = model.selectedAddress.City;
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Iyzipay.Model.Address shippingAddress = new Iyzipay.Model.Address();
            shippingAddress.ContactName = model.selectedAddress.FirstName + " " + model.selectedAddress.LastName;
            shippingAddress.City = model.selectedAddress.City;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = model.selectedAddress.Neighborhood + " " + model.selectedAddress.AddressBody + model.selectedAddress.District;
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Iyzipay.Model.Address billingAddress = new Iyzipay.Model.Address();
            billingAddress.ContactName = model.selectedAddress.FirstName + " " + model.selectedAddress.LastName;
            billingAddress.City = model.selectedAddress.City;
            billingAddress.Country = "Turkey";
            billingAddress.Description = model.selectedAddress.Neighborhood + " " + model.selectedAddress.AddressBody + model.selectedAddress.District;
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem basketItem;

            foreach (var item in model.CartModel.CartItems)
            {
                basketItem = new BasketItem();
                basketItem.Id = item.ProductId.ToString();
                basketItem.Name = item.Name;
                basketItem.Category1 = "Telefon";
                basketItem.Price = (item.Price * item.Quantity).ToString();
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                basketItems.Add(basketItem);
            }          
            request.BasketItems = basketItems;
            return Payment.Create(request, options);            
        }

        
        public IActionResult GetOrders()
        {
            var userId = _userManager.GetUserId(User);    
            var orders =_orderService.GetOrders(userId);

            var orderListModel = new List<OrderListModel>();
            OrderListModel orderModel;
            foreach (var order in orders)
            {
                orderModel = new OrderListModel();
                orderModel.Address = new AddressModel();

                orderModel.OrderId = order.Id;
                orderModel.OrderNumber = order.OrderNumber;
                orderModel.OrderDate = order.OrderDate;
                orderModel.Address.AddressTitle = order.AddressTitle;
                orderModel.Address.FirstName = order.FirstName;
                orderModel.Address.LastName = order.LastName;
                orderModel.Email = order.Email;
                orderModel.Address.Phone = order.Phone;
                orderModel.Address.City = order.City;
                orderModel.Address.District = order.District;
                orderModel.Address.Neighborhood = order.Neighborhood;
                orderModel.Address.AddressBody = order.AddressBody;
                //orderModel.Address = order.Address.AddressBody + order.Address.District + order.Address.Neighborhood;
               
                orderModel.OrderState = order.OrderState;
                orderModel.PaymentType = order.PaymentType;

                orderModel.OrderItems = order.OrderItems.Select(i=>new OrderItemModel(){
                        OrderItemId = i.Id,
                        Name = i.Product.Name,
                        Price = (double)i.Price,
                        Quantity = i.Quantity,
                        ImageUrl = i.Product.ImageUrl
                }).ToList();

                orderListModel.Add(orderModel);
            }


            return View("Orders", orderListModel);
        }
        
    }
}