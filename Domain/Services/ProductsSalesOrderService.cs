using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Core;

namespace Domain.Services
{
    public interface IProductsSalesOrderService
    {
        Task<ProductsSalesOrder> Add(ProductsSalesOrder entity);
        Task<ProductsSalesOrder> Remove(int key);
        Task<ProductsSalesOrder> Edit(ProductsSalesOrder entity);
        IQueryable<ProductsSalesOrder> GetAll();
        Task<ProductsSalesOrder> GetSingle(int key);
    }
    public class ProductsSalesOrderService : IProductsSalesOrderService
    {
        private readonly IEntityRepository<ProductsSalesOrder> _productsSalesOrder;
        public ProductsSalesOrderService(IEntityRepository<ProductsSalesOrder> productsSalesOrder)
        {
            _productsSalesOrder = productsSalesOrder;
        }
        public Task<ProductsSalesOrder> Add(ProductsSalesOrder entity)
        {
            return _productsSalesOrder.Add(entity);
        }

        public Task<ProductsSalesOrder> Edit(ProductsSalesOrder entity)
        {
            return _productsSalesOrder.Edit(entity);
        }

        public IQueryable<ProductsSalesOrder> GetAll()
        {
            return _productsSalesOrder.GetAll();
        }        

        public Task<ProductsSalesOrder> GetSingle(int key)
        {
            return _productsSalesOrder.GetSingle(key);
        }

        public Task<ProductsSalesOrder> Remove(int key)
        {
            throw new NotImplementedException();
        }
    }
}
