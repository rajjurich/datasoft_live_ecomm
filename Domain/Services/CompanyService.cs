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
    public interface ICompanyService
    {
        Task<Company> Add(Company entity);
        Task<Company> Delete(Company entity);
        Task<Company> Edit(Company entity);
        IQueryable<Company> GetAll();
        Task<Company> GetSingle(int key);
        Task<Company> GetSingle(Company entity);
    }
    public class CompanyService : ICompanyService
    {
        private readonly IEntityRepository<Company> _companyRepository;
        public CompanyService(IEntityRepository<Company> companyRepository)
        {
            _companyRepository = companyRepository;
        }
        public Task<Company> Add(Company entity)
        {
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.Now;
            return _companyRepository.Add(entity);
        }

        public Task<Company> Delete(Company entity)
        {
            entity.IsDeleted = true;
            entity.ModifiedDate = DateTime.Now;
            return _companyRepository.Edit(entity);
        }

        public Task<Company> Edit(Company entity)
        {
            entity.ModifiedDate = DateTime.Now;
            return _companyRepository.Edit(entity);
        }

        public async Task<Company> GetSingle(Company entity)
        {
            return await _companyRepository.GetAll().Where(x => x.IsDeleted == false && (x.CompanyName == entity.CompanyName || x.PrimaryMobileNumber == entity.PrimaryMobileNumber)).FirstOrDefaultAsync();
        }

        public IQueryable<Company> GetAll()
        {
            return _companyRepository.GetAll().Where(c => c.IsDeleted == false);
        }

        public Task<Company> GetSingle(int key)
        {
            return _companyRepository.GetSingle(key);
        }
    }
}
