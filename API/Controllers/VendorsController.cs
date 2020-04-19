using API.Mappers;
using Dtos;
using Domain.Entities;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace API.Controllers
{
    [Authorize]
    public class VendorsController : ApiController
    {
        private readonly IVendorService _vendorService;
        private readonly IResourceService _resourceService;

        private readonly static Expression<Func<Vendor, VendorInfoDto>> AsVendorDto =
           x => new VendorInfoDto
           {
               VendorId = x.VendorId,
               VendorName = x.VendorName,
               EmailId = x.EmailId,
               PrimaryMobileNumber = x.PrimaryMobileNumber,
               CompanyName = x.Company.CompanyName,
               GSTNumber = x.GSTNumber,
               PAN = x.PAN,
               SecondaryMobileNumber = x.SecondaryMobileNumber
           };

        public VendorsController(IVendorService vendorService
            , IResourceService resourceService)
        {
            _vendorService = vendorService;
            _resourceService = resourceService;
        }

        // GET: api/Vendors
        public IQueryable<VendorInfoDto> GetVendors()
        {
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = userManager.GetRolesAsync(User.Identity.GetUserId()).Result;
            var user = User.Identity.Name;
            if (roles[0] == "user")
            {
                var companies = _resourceService.GetAll().Where(x => x.ResourceName == user).Select(x => x.CompanyId).ToList();
                return _vendorService.GetAll().Where(x => companies.Contains(x.CompanyId)).Select(AsVendorDto);
            }
            return _vendorService.GetAll().Select(AsVendorDto);
        }

        // GET: api/Vendors/5
        [ResponseType(typeof(VendorDto))]
        public async Task<IHttpActionResult> GetVendor(int id)
        {
            Vendor vendor = await _vendorService.GetSingle(id);

            if (vendor == null)
            {
                return NotFound();
            }

            return Ok(Mapping.ToVendorDto(vendor));
        }

        // GET: api/GetCustomerInfo/588888888        
        [ResponseType(typeof(CustomerDto))]
        [Route("api/Vendors/GetVendorInfo/{mobileNumber}")]
        public async Task<IHttpActionResult> GetCustomerInfo(long mobileNumber)
        {
            Vendor vendor = await _vendorService.GetAll().Where(x => x.PrimaryMobileNumber == mobileNumber).FirstOrDefaultAsync();

            if (vendor == null)
            {
                return NotFound();
            }

            return Ok(Mapping.ToVendorDto(vendor));
        }

        // PUT: api/Vendors/5
        [ResponseType(typeof(VendorDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> PutVendor(int id, VendorDto vendorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vendorDto.VendorId)
            {
                return BadRequest("Mismatch");
            }
            var vendor = await _vendorService.GetSingle(id);

            if (vendor == null)
            {
                return NotFound();
            }
            var updatedVendor = await _vendorService.Edit(Mapping.ToVendor(vendor, vendorDto));

            return Ok(Mapping.ToVendorDto(updatedVendor));            
        }

        // POST: api/Vendors
        [ResponseType(typeof(VendorDto))]
        public async Task<IHttpActionResult> PostVendor(VendorDto vendorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newVendor = new Vendor();

            var vendor = await _vendorService.Add(Mapping.ToVendor(newVendor, vendorDto));

            return CreatedAtRoute("DefaultApi", new { id = vendor.VendorId }, Mapping.ToVendorDto(vendor));            
        }

        // DELETE: api/Vendors/5
        [ResponseType(typeof(VendorDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> DeleteVendor(int id)
        {
            Vendor vendor = await _vendorService.GetSingle(id);

            Vendor deletedVendor = await _vendorService.Delete(vendor);

            return Ok(Mapping.ToVendorDto(deletedVendor));            
        }
    }
}
