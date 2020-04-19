using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Core;

namespace Domain.Services
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrder> Add(PurchaseOrder entity);
        Task<PurchaseOrder> Delete(PurchaseOrder entity);
        Task<PurchaseOrder> Edit(PurchaseOrder entity);
        IQueryable<PurchaseOrder> GetAll();
        Task<PurchaseOrder> GetSingle(int key);
    }
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IEntityRepository<PurchaseOrder> _purchaseOrderRepository;

        public PurchaseOrderService(IEntityRepository<PurchaseOrder> purchaseOrderRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
        }
        public Task<PurchaseOrder> Add(PurchaseOrder entity)
        {
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.Now;
            return _purchaseOrderRepository.Add(entity);
        }

        public Task<PurchaseOrder> Delete(PurchaseOrder entity)
        {
            entity.IsDeleted = true;
            entity.ModifiedDate = DateTime.Now;
            return _purchaseOrderRepository.Edit(entity);
        }

        public Task<PurchaseOrder> Edit(PurchaseOrder entity)
        {
            entity.ModifiedDate = DateTime.Now;
            return _purchaseOrderRepository.Edit(entity);
        }

        public IQueryable<PurchaseOrder> GetAll()
        {
            return _purchaseOrderRepository.GetAll().Where(x => x.IsDeleted == false);
        }

        public Task<PurchaseOrder> GetSingle(int key)
        {
            return _purchaseOrderRepository.GetSingle(key);
        }
    }
}
