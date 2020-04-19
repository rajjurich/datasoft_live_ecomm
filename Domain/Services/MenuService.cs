using Domain.Core;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IMenuService
    {
        Task<Menu> Add(Menu entity);
        Task<Menu> Delete(Menu entity);
        Task<Menu> Edit(Menu entity);
        IQueryable<Menu> GetAll(string roleName);
        IQueryable<Menu> GetAll();
        Task<Menu> GetSingle(int key);
        Task<Menu> GetSingle(string key);
    }
    public class MenuService : IMenuService
    {
        private readonly IEntityRepository<Menu> _menuRepository;

        public MenuService(IEntityRepository<Menu> menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public Task<Menu> Add(Menu entity)
        {
            return _menuRepository.Add(entity);
        }

        public Task<Menu> Delete(Menu entity)
        {
            return _menuRepository.Edit(entity);
        }

        public Task<Menu> Edit(Menu entity)
        {
            return _menuRepository.Edit(entity);
        }

        public IQueryable<Menu> GetAll()
        {
            return _menuRepository.GetAll();
        }

        public IQueryable<Menu> GetAll(string roleName)
        {
            //var studentRep = new Repository<Student>(ctx);
            //var standardRep = new Repository<Standard>(ctx);
            //var studentToStandard = studentRep.GetAll().Join(standardRep.GetAll(),
            //                      student => student.StandardRefId,
            //                      standard => standard.StandardId,
            //                      (stud, stand) => new { Student = stud, Standard = stand }).ToList();
            var val = _menuRepository.GetAll()
                .Include(x => x.Submenus.Select(a=>a.Submenus))
                .Include(x => x.MenuAccesses)
                .Include(x => x.MenuAccesses.Select(y => y.Role))
                .Where(x => x.MenuAccesses.Any(z => z.Role.RoleName == roleName));


            return val;
        }

        public async Task<Menu> GetSingle(int key)
        {
            return await _menuRepository.GetSingle(key);
            //IQueryable<HomeViewModel> test = db.Questions
            //                      .Where(x => x.CategoriesID == categoryId)
            //                      .Select(q => q.ToHomeViewModel(User.Identity.GetUserId()));
            var val = await _menuRepository.GetAll()
                .Include(x => x.Submenus)
                .Where(x => x.MenuAccesses.Where(z => z.MenuId == key).Select(z => z.Role.RoleName == "superadmin").FirstOrDefault())
                .Where(x => x.MenuId == key)
                .FirstOrDefaultAsync();
            return val;
        }

        public async Task<Menu> GetSingle(string name)
        {
            return await _menuRepository.GetAll().Where(x => x.Title == name).FirstOrDefaultAsync();
        }
    }
}
