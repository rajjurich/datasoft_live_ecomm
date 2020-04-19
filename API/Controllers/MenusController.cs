using API.Mappers;
using Dtos;
using Domain.Entities;
using Domain.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace API.Controllers
{
    //[Authorize(Roles = "superadmin")]
    [Authorize]
    public class MenusController : ApiController
    {
        private readonly IMenuService _menuService;

        //private static readonly Expression<Func<Menu, MenuDto>> AsMenuDto =
        //    x => new MenuDto
        //    {
        //        MenuId = x.MenuId,
        //        Title = x.Title,
        //        Icon = x.Icon,
        //        URL = x.URL,
        //        Submenus = x.Submenu.Select(y => y.Submenu).ToList()
        //    };
        public MenusController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        // GET: api/Menus        
        public IQueryable<Menu> GetMenus()
        {
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = userManager.GetRolesAsync(User.Identity.GetUserId()).Result;
            var user = User.Identity.Name;
            //var x = User.IsInRole(roles[0]);
            var z = _menuService.GetAll(roles[0]);           
            return z;
        }

        // GET: api/Menus/all        
        [Route("api/Menus/all")]
        public IQueryable<Menu> GetAllMenus()
        {
           // var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var roles = userManager.GetRolesAsync(User.Identity.GetUserId()).Result;
            //var x = User.IsInRole(roles[0]);
            var z = _menuService.GetAll();
            return z;
        }

        // GET: api/Menus/5
        [ResponseType(typeof(Menu))]
        public async Task<IHttpActionResult> GetMenu(int id)
        {
            Menu menu = await _menuService.GetSingle(id);
            if (menu == null)
            {
                return NotFound();
            }

            return Ok(menu);
        }

        // PUT: api/Menus/5
        [ResponseType(typeof(Menu))]
        public async Task<IHttpActionResult> PutMenu(int id, Menu menu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != menu.MenuId)
            {
                return BadRequest("Mismatch");
            }

            var updatedMenu = await _menuService.Edit(menu);

            return Ok(updatedMenu);
        }

        // POST: api/Menus
        [ResponseType(typeof(Menu))]
        public async Task<IHttpActionResult> PostMenu(Menu menu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getMenu = await _menuService.Add(menu);

            return CreatedAtRoute("DefaultApi", new { id = getMenu.MenuId }, getMenu);
        }

        // DELETE: api/Menus/5
        [ResponseType(typeof(Menu))]
        public async Task<IHttpActionResult> DeleteMenu(int id)
        {
            Menu menu = await _menuService.GetSingle(id);
            Menu deletedMenu = await _menuService.Delete(menu);

            return Ok(deletedMenu);
        }
    }
}
