using Domain.Core;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IRoleService
    {
        IQueryable<Role> GetAll();
        Task<Role> GetSingle(int key);
    }
    public class RoleService : IRoleService
    {
        private readonly IEntityRepository<Role> _roleRepository;
        public RoleService(IEntityRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public IQueryable<Role> GetAll()
        {
            return _roleRepository.GetAll().Where(x => x.RoleName != "superadmin");
        }

        public Task<Role> GetSingle(int key)
        {
            return _roleRepository.GetSingle(key);
        }
    }
}
