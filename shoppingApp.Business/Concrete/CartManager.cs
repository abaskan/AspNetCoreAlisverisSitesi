using shoppingApp.Business.Abstract;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.DataAccess.Concrete.EFCore;
using shoppingApp.Entity;

namespace shoppingApp.Business.Concrete
{
    public class CartManager : ICartService
    {
        private IUnitOfWork _unitOfWork;
        public CartManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AddToCart(string userId, int productId, int quantity)
        {
            var cart = GetCartByUserId(userId);

            if(cart!=null)
            {
                // eklenmek isteyen ürün sepette varmı (güncelleme)
                // eklenmek isteyen ürün sepette var ve yeni kayıt oluştur. (kayıt ekleme)

                var index = cart.CartItems.FindIndex(i=>i.ProductId==productId);                
                if(index<0)
                {
                    cart.CartItems.Add(new CartItem(){
                        ProductId = productId,
                        Quantity = quantity,
                        CartId = cart.Id
                    });
                }
                else{
                    cart.CartItems[index].Quantity += quantity;
                }

                _unitOfWork.CartRepository.Update(cart);
                _unitOfWork.Save();

            }
        }

        public void ClearCart(int cartId)
        {
            _unitOfWork.CartRepository.ClearCart(cartId);
            _unitOfWork.Save();
        }

        public void DeleteFromCart(string userId, int productId)
        {
            var cart = GetCartByUserId(userId);

            if(cart != null)
            {
                _unitOfWork.CartRepository.DeleteFromCart(cart.Id,productId);
                _unitOfWork.Save();
            }
        }

        public Cart GetCartByUserId(string userId)
        {
            return _unitOfWork.CartRepository.GetByUserId(userId);
        }

        public void InitializeCart(string userId)
        {
            _unitOfWork.CartRepository.Create(new Cart(){UserId = userId});
            _unitOfWork.Save();
        }

        public void UpdateQuantity(string userId, int productId, int quantity)
        {
            var cart = GetCartByUserId(userId);

            if(cart!=null)
            {
                var index = cart.CartItems.FindIndex(i=>i.ProductId==productId);                
                
                cart.CartItems[index].Quantity = quantity;

                _unitOfWork.CartRepository.Update(cart);
                _unitOfWork.Save();
            }
        }
    }
}