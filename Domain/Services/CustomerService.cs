using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Core;
using System.Data.Entity;

namespace Domain.Services
{
    public interface ICustomerService
    {
        Task<Customer> Add(Customer entity);
        Task<Customer> Delete(Customer entity);
        Task<Customer> Edit(Customer entity);
        IQueryable<Customer> GetAll();
        Task<Customer> GetSingle(int key);
        Task<Customer> GetSingle(Customer entity);
    }
    public class CustomerService : ICustomerService
    {
        private readonly IEntityRepository<Customer> _customerRepository;

        public CustomerService(IEntityRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public Task<Customer> Add(Customer entity)
        {
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.Now;
            return _customerRepository.Add(entity);
        }

        public Task<Customer> Delete(Customer entity)
        {
            entity.IsDeleted = true;
            entity.ModifiedDate = DateTime.Now;
            return _customerRepository.Edit(entity);
        }

        public Task<Customer> Edit(Customer entity)
        {
            entity.ModifiedDate = DateTime.Now;
            return _customerRepository.Edit(entity);
        }

        public IQueryable<Customer> GetAll()
        {
            return _customerRepository.GetAll().Where(x => x.IsDeleted == false);
        }

        public async Task<Customer> GetSingle(Customer entity)
        {
            return await _customerRepository.GetAll().Where(x => x.IsDeleted == false && (x.CustomerName == entity.CustomerName || x.PrimaryMobileNumber == entity.PrimaryMobileNumber)).FirstOrDefaultAsync();
        }

        public Task<Customer> GetSingle(int key)
        {
            return _customerRepository.GetSingle(key);
        }
    }
}
