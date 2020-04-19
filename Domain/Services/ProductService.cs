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
    public interface IProductService
    {
        Task<Product> Add(Product entity);
        Task<Product> Delete(Product entity);
        Task<Product> Edit(Product entity);
        IQueryable<Product> GetAll();        
        Task<Product> GetSingle(int key);        
    }
    public class ProductService : IProductService
    {
        private readonly IEntityRepository<Product> _productRepository;

        public ProductService(IEntityRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> Add(Product entity)
        {
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.Now;
            var x = await _productRepository.Add(entity);
            return x;
        }

        public async Task<Product> Delete(Product entity)
        {
            entity.IsDeleted = true;
            entity.ModifiedDate = DateTime.Now;
            return await _productRepository.Edit(entity);
        }

        public async Task<Product> Edit(Product entity)
        {
            entity.ModifiedDate = DateTime.Now;
            return await _productRepository.Edit(entity);
        }

        public IQueryable<Product> GetAll()
        {
            return _productRepository.GetAll().Where(x => x.IsDeleted == false);
        }        

        public async Task<Product> GetSingle(int key)
        {
            return await _productRepository.GetSingle(key);
        }
        
    }
}
