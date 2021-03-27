using System.Collections.Generic;
using System.Linq;

namespace shoppingApp.WebUI.Models
{
    public class CartModel
    {
        public int CartId { get; set; }
        public double ShippingFee { get; set; } = 9.99;
        public List<CartItemModel> CartItems { get; set; }

        public double TotalPrice()
        {
            return CartItems.Sum(i=>i.Price*i.Quantity) + this.ShippingFee;
        }

        public int TotalItem()
        {
            return CartItems.Sum(i => i.Quantity);
        }
    }

    public class CartItemModel 
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
    }


}