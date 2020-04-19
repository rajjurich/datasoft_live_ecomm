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
    public interface IResourceService
    {
        Task<Resource> Add(Resource entity);
        Task<Resource> Delete(Resource entity);
        Task<Resource> Edit(Resource entity);
        IQueryable<Resource> GetAll();
        Task<Resource> GetSingle(int key);
        Task<Resource> GetSingle(string key);
    }
    public class ResourceService : IResourceService
    {
        private readonly IEntityRepository<Resource> _resourceRepository;

        public ResourceService(IEntityRepository<Resource> resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }
        public Task<Resource> Add(Resource entity)
        {
            return _resourceRepository.Add(entity);
        }

        public Task<Resource> Edit(Resource entity)
        {
            return _resourceRepository.Edit(entity);
        }

        public IQueryable<Resource> GetAll()
        {
            return _resourceRepository.GetAll().Where(x => x.IsDeleted == false);
        }

        public Task<Resource> GetSingle(int key)
        {
            return _resourceRepository.GetSingle(key);
        }

        public async Task<Resource> GetSingle(string name)
        {
            return await _resourceRepository.GetAll().Where(x => x.ResourceName == name).FirstOrDefaultAsync();
        }

        public Task<Resource> Delete(Resource entity)
        {
            entity.IsDeleted = true;
            return _resourceRepository.Edit(entity);
        }
    }
}
