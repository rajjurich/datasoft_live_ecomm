using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Core;

namespace Domain.Services
{
    public interface ISalesOrderService
    {
        Task<SalesOrder> Add(SalesOrder entity);
        Task<SalesOrder> Delete(SalesOrder entity);
        Task<SalesOrder> Edit(SalesOrder entity);
        IQueryable<SalesOrder> GetAll();
        Task<SalesOrder> GetSingle(int key);
    }
    public class SalesOrderService : ISalesOrderService
    {
        private readonly IEntityRepository<SalesOrder> _salesOrderRepository;

        public SalesOrderService(IEntityRepository<SalesOrder> salesOrderRepository)
        {
            _salesOrderRepository = salesOrderRepository;
        }

        public Task<SalesOrder> Add(SalesOrder entity)
        {
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.Now;
            return _salesOrderRepository.Add(entity);
        }

        public Task<SalesOrder> Delete(SalesOrder entity)
        {
            entity.IsDeleted = true;
            entity.ModifiedDate = DateTime.Now;
            return _salesOrderRepository.Edit(entity);
        }

        public Task<SalesOrder> Edit(SalesOrder entity)
        {
            entity.ModifiedDate = DateTime.Now;
            return _salesOrderRepository.Edit(entity);
        }

        public IQueryable<SalesOrder> GetAll()
        {
            return _salesOrderRepository.GetAll().Where(x => x.IsDeleted == false);
        }

        public Task<SalesOrder> GetSingle(int key)
        {
            return _salesOrderRepository.GetSingle(key);
        }
    }
}
