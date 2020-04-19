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
    public class CustomersController : ApiController
    {
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly IResourceService _resourceService;

        private readonly static Expression<Func<Customer, CustomerInfoDto>> AsCustomerDto =
           x => new CustomerInfoDto
           {
               CustomerId = x.CustomerId,
               CustomerName = x.CustomerName,
               EmailId = x.EmailId,
               PrimaryMobileNumber = x.PrimaryMobileNumber,
               CompanyName = x.Company.CompanyName,
               GSTNumber = x.GSTNumber,
               PAN = x.PAN,
               SecondaryMobileNumber = x.SecondaryMobileNumber
           };

        public CustomersController(ICustomerService customerService, IAddressService addressService
            , IResourceService resourceService)
        {
            _customerService = customerService;
            _addressService = addressService;
            _resourceService = resourceService;
        }

        // GET: api/Customers
        public IQueryable<CustomerInfoDto> GetCustomers()
        {
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = userManager.GetRolesAsync(User.Identity.GetUserId()).Result;
            var user = User.Identity.Name;
            if (roles[0] == "user")
            {
                var companies = _resourceService.GetAll().Where(x => x.ResourceName == user).Select(x => x.CompanyId).ToList();
                return _customerService.GetAll().Where(x => companies.Contains(x.CompanyId)).Select(AsCustomerDto);
            }
            return _customerService.GetAll().Select(AsCustomerDto);
        }

        // GET: api/Customers/5
        [ResponseType(typeof(CustomerDto))]
        public async Task<IHttpActionResult> GetCustomer(int id)
        {
            Customer customer = await _customerService.GetSingle(id);

            var addresses = _addressService.GetAllByCustomerId(customer.CustomerId).ToList();

            customer.Addresses = addresses;

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(Mapping.ToCustomerDto(customer));
        }

        // GET: api/GetCustomerInfo/588888888        
        [ResponseType(typeof(CustomerDto))]
        [Route("api/Customers/GetCustomerInfo/{mobileNumber}")]
        public async Task<IHttpActionResult> GetCustomerInfo(long mobileNumber)
        {
            Customer customer = await _customerService.GetAll().Where(x => x.PrimaryMobileNumber == mobileNumber).FirstOrDefaultAsync();
            var addresses = _addressService.GetAllByCustomerId(customer.CustomerId).ToList();

            customer.Addresses = addresses;
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(Mapping.ToCustomerDto(customer));
        }

        // GET: api/GetCustomerMobileNumberList        
        [ResponseType(typeof(CustomerDto))]
        [Route("api/Customers/GetCustomerMobileNumberList")]
        public IQueryable<long> GetCustomerMobileNumberList()
        {
            var customerMobileNumberList = _customerService.GetAll().Where(m => m.IsDeleted == false).Select(m => m.PrimaryMobileNumber);

            return customerMobileNumberList;
        }

        // PUT: api/Customers/5
        [ResponseType(typeof(CustomerDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> PutCustomer(int id, CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerDto.CustomerId)
            {
                return BadRequest("Mismatch");
            }
            var customer = await _customerService.GetSingle(id);

            if (customer == null)
            {
                return NotFound();
            }
            Customer updatedCustomer = await _customerService.Edit(Mapping.ToCustomer(customer, customerDto));
            var addresses = _addressService.GetAllByCustomerId(customer.CustomerId).ToList();

            updatedCustomer.Addresses = addresses;

            foreach (var item in customerDto.Addresses)
            {
                
                await _addressService.Edit(Mapping.ToAddress(updatedCustomer.Addresses.FirstOrDefault(),item));
            }

          

            return Ok(Mapping.ToCustomerDto(updatedCustomer));
        }

        // POST: api/Customers
        [ResponseType(typeof(CustomerDto))]
        public async Task<IHttpActionResult> PostCustomer(CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newCustomer = new Customer();

            var customer = await _customerService.Add(Mapping.ToCustomer(newCustomer, customerDto));

            foreach (var item in customerDto.Addresses)
            {
                Address address = new Address();
                address.CustomerId = customer.CustomerId;
                address.FullAddress = item.FullAddress;
                address.StateId = item.StateId;
                address.DistrictId = item.DistrictId;
                await _addressService.Add(address);
            }

            var addresses = _addressService.GetAllByCustomerId(customer.CustomerId).ToList();

            customer.Addresses = addresses;

            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, Mapping.ToCustomerDto(customer));
        }

        // DELETE: api/Customers/5
        [ResponseType(typeof(CustomerInfoDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> DeleteCustomer(int id)
        {
            Customer customer = await _customerService.GetSingle(id);

            Customer deletedCustomer = await _customerService.Delete(customer);

            return Ok(Mapping.ToCustomerDto(deletedCustomer));
        }


    }
}