using System.Collections.Generic;
using System.Linq;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Concrete.EFCore
{
    public class EFCoreAddressRepository : EfCoreGenericRepository<Address>
                                        , IAddressRepository
    {

        public EFCoreAddressRepository(ShoppingContext context) : base(context)
        {
            
        }
        private ShoppingContext ShoppingContext
        {
            get { return context as ShoppingContext; }
        }


        public List<Address> GetByUserId(string userId)
        {
           
            return ShoppingContext.Addresses
                    .Where(i => i.UserId == userId)
                    .ToList();
        }
    }
}