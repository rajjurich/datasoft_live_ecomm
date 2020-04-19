using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Domain.Core;
using Domain.Entities;
using Domain.Services;
using System.Linq.Expressions;
using Dtos;
using API.Mappers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace API.Controllers
{
    [Authorize]
    public class ResourcesController : ApiController
    {
        private readonly IResourceService _resourceService;

        private static readonly Expression<Func<Resource, ResourceInfoDto>> AsResourceDto =
            x => new ResourceInfoDto
            {
                ResourceId = x.ResourceId,
                ResourceName = x.ResourceName,
                Role = x.Role.RoleName,
                RoleId = x.RoleId,
                CompanyName = x.Company.CompanyName,
                CompanyId = x.CompanyId
            };
        public ResourcesController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        // GET: api/Resources
        public IQueryable<ResourceInfoDto> GetResources()
        {
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = userManager.GetRolesAsync(User.Identity.GetUserId()).Result;
            var user = User.Identity.Name;
            if (roles[0] == "user")
            {
                var companies = _resourceService.GetAll().Where(x => x.ResourceName == user).Select(x => x.CompanyId).ToList();
                return _resourceService.GetAll().Where(x => companies.Contains(x.CompanyId) && x.ResourceName == user).Select(AsResourceDto);
            }
            return _resourceService.GetAll().Select(AsResourceDto);
        }

        // GET: api/Resources/5
        [ResponseType(typeof(ResourceInfoDto))]
        public async Task<IHttpActionResult> GetResource(int id)
        {
            Resource resource = await _resourceService.GetSingle(id);
            if (resource == null)
            {
                return NotFound();
            }

            return Ok(Mapping.ToResourceDto(resource));
        }

        // PUT: api/Resources/5
        [ResponseType(typeof(ResourceDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> PutResource(int id, ResourceDto resourceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != resourceDto.ResourceId)
            {
                return BadRequest("Mismatch");
            }

            var updatedResource = await _resourceService.Edit(Mapping.ToResource(resourceDto));

            return Ok(Mapping.ToResourceDto(updatedResource));
        }

        // POST: api/Resources
        [ResponseType(typeof(ResourceDto))]
        public async Task<IHttpActionResult> PostResource(ResourceDto resourceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resource = await _resourceService.Add(Mapping.ToResource(resourceDto));

            return CreatedAtRoute("DefaultApi", new { id = resource.ResourceId }, Mapping.ToResourceDto(resource));
        }

        // DELETE: api/Resources/5
        [ResponseType(typeof(ResourceDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> DeleteResource(int id)
        {
            Resource resource = await _resourceService.GetSingle(id);

            Resource deletedResource = await _resourceService.Delete(resource);

            return Ok(Mapping.ToResourceDto(deletedResource));
        }

    }
}