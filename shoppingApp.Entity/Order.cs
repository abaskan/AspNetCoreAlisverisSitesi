using System;
using System.Collections.Generic;

namespace shoppingApp.Entity
{
    public class Order
    {       
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string PaymentId { get; set; }
        public string ConversationId { get; set; }
        public EnumPaymentType PaymentType { get; set; }
        public EnumOrderState OrderState { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public string AddressTitle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Neighborhood { get; set; }
        public string AddressBody { get; set; }
    }

    public enum EnumPaymentType
    {
        CreditCard=0,
        Eft=1
    }

    public enum EnumOrderState
    {
        waiting=0,
        unpaid=1,
        completed=2
    } 
}