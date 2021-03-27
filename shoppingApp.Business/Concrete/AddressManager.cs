using System.Collections.Generic;
using shoppingApp.Business.Abstract;
using shoppingApp.DataAccess.Abstract;
using shoppingApp.Entity;
using System.Threading.Tasks;

namespace shoppingApp.Business.Concrete
{
    public class AddressManager : IAddressService
    {
        private IUnitOfWork _unitOfWork;

        public AddressManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void Create(Address entity)
        {
            _unitOfWork.AddressRepository.Create(entity);
            _unitOfWork.Save();
        }

        public void Delete(Address entity)
        {
            _unitOfWork.AddressRepository.Delete(entity);
            _unitOfWork.Save();
        }

        //public List<Address> GetAll()
        //{
        //    return _unitOfWork.AddressRepository.GetAll();
        //}

        public async Task<Address> GetById(int id)
        {
            return await _unitOfWork.AddressRepository.GetById(id);
        }

        public List<Address> GetByUserId(string userId)
        {
            return _unitOfWork.AddressRepository.GetByUserId(userId);
        }

        public void Update(Address entity)
        {
            _unitOfWork.AddressRepository.Update(entity);
            _unitOfWork.Save();
        }
    }
}