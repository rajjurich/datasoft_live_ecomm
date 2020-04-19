using Domain.Core;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IProductsPurchaseOrderService
    {
        Task<ProductsPurchaseOrder> Add(ProductsPurchaseOrder entity);
        Task<ProductsPurchaseOrder> Remove(int key);
        Task<ProductsPurchaseOrder> Edit(ProductsPurchaseOrder entity);
        IQueryable<ProductsPurchaseOrder> GetAll();
        Task<ProductsPurchaseOrder> GetSingle(int key);
    }
    public class ProductsPurchaseOrderService : IProductsPurchaseOrderService
    {
        private readonly IEntityRepository<ProductsPurchaseOrder> _productsSalesOrder;
        public ProductsPurchaseOrderService(IEntityRepository<ProductsPurchaseOrder> productsSalesOrder)
        {
            _productsSalesOrder = productsSalesOrder;
        }
        public Task<ProductsPurchaseOrder> Add(ProductsPurchaseOrder entity)
        {
            return _productsSalesOrder.Add(entity);
        }

        public Task<ProductsPurchaseOrder> Edit(ProductsPurchaseOrder entity)
        {
            return _productsSalesOrder.Edit(entity);
        }

        public IQueryable<ProductsPurchaseOrder> GetAll()
        {
            return _productsSalesOrder.GetAll();
        }

        public Task<ProductsPurchaseOrder> GetSingle(int key)
        {
            return _productsSalesOrder.GetSingle(key);
        }

        public Task<ProductsPurchaseOrder> Remove(int key)
        {
            throw new NotImplementedException();
        }
    }
}
