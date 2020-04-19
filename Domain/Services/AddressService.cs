using Domain.Core;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAddressService
    {
        Task<Address> Add(Address entity);
        Task<Address> Delete(Address entity);
        Task<Address> Edit(Address entity);
        IQueryable<Address> GetAll();
        IQueryable<Address> GetAllByCustomerId(int customerId);
        Task<Address> GetSingle(int key);
        Task<Address> GetSingle(string key);
    }
    public class AddressService : IAddressService
    {
        private readonly IEntityRepository<Address> _categoryRepository;
        public AddressService(IEntityRepository<Address> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public Task<Address> Add(Address entity)
        {
            return _categoryRepository.Add(entity);
        }

        public Task<Address> Delete(Address entity)
        {
            throw new NotImplementedException();
        }

        public Task<Address> Edit(Address entity)
        {
            return _categoryRepository.Edit(entity);
        }

        public IQueryable<Address> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Address> GetAllByCustomerId(int customerId)
        {
            return _categoryRepository.GetAll().Where(x => x.CustomerId == customerId);
        }

        public Task<Address> GetSingle(string key)
        {
            throw new NotImplementedException();
        }

        public Task<Address> GetSingle(int key)
        {
            throw new NotImplementedException();
        }
    }
}
