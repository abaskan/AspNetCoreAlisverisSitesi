using System.Collections.Generic;
using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Abstract
{
    public interface IAddressRepository : IRepository<Address>
    {
        List<Address> GetByUserId(string userId);
    }
}