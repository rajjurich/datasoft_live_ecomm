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
    public interface ICategoryService
    {
        Task<Category> Add(Category entity);
        Task<Category> Delete(Category entity);
        Task<Category> Edit(Category entity);
        IQueryable<Category> GetAll();
        Task<Category> GetSingle(int key);
        Task<Category> GetSingle(string key);
    }
    public class CategoryService : ICategoryService
    {
        private readonly IEntityRepository<Category> _addressRepository;
        public CategoryService(IEntityRepository<Category> addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public Task<Category> Add(Category entity)
        {
            return _addressRepository.Add(entity);
        }

        public Task<Category> Delete(Category entity)
        {
            entity.IsDeleted = true;
            return _addressRepository.Edit(entity);
        }

        public Task<Category> Edit(Category entity)
        {
            return _addressRepository.Edit(entity);
        }

        public IQueryable<Category> GetAll()
        {
            return _addressRepository.GetAll().Where(x => x.IsDeleted == false);
        }

        public Task<Category> GetSingle(int key)
        {
            return _addressRepository.GetSingle(key);
        }

        public async Task<Category> GetSingle(string name)
        {
            return await _addressRepository.GetAll().Where(x => x.CategoryName == name).FirstOrDefaultAsync();
        }
        
    }
}
