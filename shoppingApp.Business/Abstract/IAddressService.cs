using System.Collections.Generic;
using shoppingApp.Entity;
using System.Threading.Tasks;

namespace shoppingApp.Business.Abstract
{
    public interface IAddressService
    {
        Task<Address> GetById(int id);
        List<Address> GetByUserId(string userId);
        //List<Address> GetAll();
        void Create(Address entity);
        void Update(Address entity);
        void Delete(Address entity);
    }
}