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
    public interface IProductTypeService
    {
        Task<ProductType> Add(ProductType entity);
        Task<ProductType> Delete(ProductType entity);
        Task<ProductType> Edit(ProductType entity);
        IQueryable<ProductType> GetAll();
        Task<ProductType> GetSingle(int key);
        Task<ProductType> GetSingle(string key);
    }
    public class ProductTypeService : IProductTypeService
    {
        private readonly IEntityRepository<ProductType> _productTypeRepository;

        public ProductTypeService(IEntityRepository<ProductType> productTypeRepository)
        {
            _productTypeRepository = productTypeRepository;
        }

        public Task<ProductType> Add(ProductType entity)
        {
            return _productTypeRepository.Add(entity);
        }

        public Task<ProductType> Delete(ProductType entity)
        {
            entity.IsDeleted = true;
            return _productTypeRepository.Edit(entity);
        }

        public Task<ProductType> Edit(ProductType entity)
        {
            return _productTypeRepository.Edit(entity);
        }

        public IQueryable<ProductType> GetAll()
        {
            return _productTypeRepository.GetAll().Where(x => x.IsDeleted == false);
        }

        public Task<ProductType> GetSingle(int key)
        {
            return _productTypeRepository.GetSingle(key);
        }

        public async Task<ProductType> GetSingle(string name)
        {
            return await _productTypeRepository.GetAll().Where(x => x.ProductTypeName == name).FirstOrDefaultAsync();
        }
        
    }
}
