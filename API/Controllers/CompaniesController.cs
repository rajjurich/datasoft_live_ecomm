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
using System.Linq.Expressions;
using Dtos;
using Domain.Services;
using API.Mappers;
using Microsoft.AspNet.Identity.Owin;
using API;
using Microsoft.AspNet.Identity;

namespace Controllers
{
    [Authorize]
    public class CompaniesController : ApiController
    {
        private readonly ICompanyService _companyService;
        private readonly IResourceService _resourceService;

        private static readonly Expression<Func<Company, CompanyDto>> AsCompanyDto =
            x => new CompanyDto
            {
                CompanyId = x.CompanyId,
                CompanyName = x.CompanyName,
                EmailId = x.EmailId,
                PrimaryMobileNumber = x.PrimaryMobileNumber
            };
        public CompaniesController(ICompanyService companyService, IResourceService resourceService)
        {
            _companyService = companyService;
            _resourceService = resourceService;
        }
        // GET: api/Companies
        public IQueryable<CompanyDto> GetCompanies()
        {
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = userManager.GetRolesAsync(User.Identity.GetUserId()).Result;
            var user = User.Identity.Name;
            if (roles[0] == "user")
            {
                var companies = _resourceService.GetAll().Where(x => x.ResourceName == user).Select(x => x.CompanyId).ToList();
                return _companyService.GetAll().Where(x => companies.Contains(x.CompanyId)).Select(AsCompanyDto);
            }
            return _companyService.GetAll().Select(AsCompanyDto);
        }

        // GET: api/Companies/5
        [ResponseType(typeof(CompanyDto))]
        public async Task<IHttpActionResult> GetCompany(int id)
        {
            Company company = await _companyService.GetSingle(id);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(Mapping.ToCompanyDto(company));
        }

        // PUT: api/Companies/5
        [ResponseType(typeof(CompanyDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> PutCompany(int id, CompanyDto companyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != companyDto.CompanyId)
            {
                return BadRequest("Mismatch");
            }
            var company = await _companyService.GetSingle(id);

            if (company == null)
            {
                return NotFound();
            }
            var updatedCompany = await _companyService.Edit(Mapping.ToCompany(company, companyDto));

            return Ok(Mapping.ToCompanyDto(updatedCompany));
        }

        // POST: api/Companies
        [ResponseType(typeof(CompanyDto))]
        public async Task<IHttpActionResult> PostCompany(CompanyDto companyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newCompany = new Company();

            var company = await _companyService.Add(Mapping.ToCompany(newCompany, companyDto));

            return CreatedAtRoute("DefaultApi", new { id = company.CompanyId }, Mapping.ToCompanyDto(company));
        }

        // DELETE: api/Companies/5
        [ResponseType(typeof(CompanyDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> DeleteCompany(int id)
        {
            Company company = await _companyService.GetSingle(id);

            Company deletedCompany = await _companyService.Delete(company);

            return Ok(Mapping.ToCompanyDto(deletedCompany));
        }

    }
}