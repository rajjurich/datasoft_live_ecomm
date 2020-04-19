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
    public interface IVendorService
    {
        Task<Vendor> Add(Vendor entity);
        Task<Vendor> Delete(Vendor entity);
        Task<Vendor> Edit(Vendor entity);
        IQueryable<Vendor> GetAll();
        Task<Vendor> GetSingle(int key);
        Task<Vendor> GetSingle(Vendor entity);
    }
    public class VendorService : IVendorService
    {
        private readonly IEntityRepository<Vendor> _vendorRepository;

        public VendorService(IEntityRepository<Vendor> vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }
        public Task<Vendor> Add(Vendor entity)
        {
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.Now;
            return _vendorRepository.Add(entity);
        }

        public Task<Vendor> Delete(Vendor entity)
        {
            entity.IsDeleted = true;
            entity.ModifiedDate = DateTime.Now;
            return _vendorRepository.Edit(entity);
        }

        public Task<Vendor> Edit(Vendor entity)
        {
            entity.ModifiedDate = DateTime.Now;
            return _vendorRepository.Edit(entity);
        }

        public IQueryable<Vendor> GetAll()
        {
            return _vendorRepository.GetAll().Where(x => x.IsDeleted == false);
        }

        public Task<Vendor> GetSingle(int key)
        {
            return _vendorRepository.GetSingle(key);
        }
        public async Task<Vendor> GetSingle(Vendor entity)
        {
            return await _vendorRepository.GetAll().Where(x => x.IsDeleted == false && (x.VendorName == entity.VendorName || x.PrimaryMobileNumber == entity.PrimaryMobileNumber)).FirstOrDefaultAsync();
        }
    }
}
