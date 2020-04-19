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
    public interface IManufacturerService
    {
        Task<Manufacturer> Add(Manufacturer entity);
        Task<Manufacturer> Delete(Manufacturer entity);
        Task<Manufacturer> Edit(Manufacturer entity);
        IQueryable<Manufacturer> GetAll();
        Task<Manufacturer> GetSingle(int key);
        Task<Manufacturer> GetSingle(string key);
    }
    public class ManufacturerService : IManufacturerService
    {
        private readonly IEntityRepository<Manufacturer> _manufacturerRepository;

        public ManufacturerService(IEntityRepository<Manufacturer> manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public Task<Manufacturer> Add(Manufacturer entity)
        {
            return _manufacturerRepository.Add(entity);
        }

        public Task<Manufacturer> Delete(Manufacturer entity)
        {
            entity.IsDeleted = true;
            return _manufacturerRepository.Edit(entity);
        }

        public Task<Manufacturer> Edit(Manufacturer entity)
        {
            return _manufacturerRepository.Edit(entity);
        }

        public IQueryable<Manufacturer> GetAll()
        {
            return _manufacturerRepository.GetAll().Where(x => x.IsDeleted == false);
        }

        public Task<Manufacturer> GetSingle(int key)
        {
            return _manufacturerRepository.GetSingle(key);
        }

        public async Task<Manufacturer> GetSingle(string name)
        {
            return await _manufacturerRepository.GetAll().Where(x => x.ManufacturerName == name).FirstOrDefaultAsync();
        }
        
    }
}
