using Dtos;
using ViewModels;
using Client;
using Web.Common;
using Web.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;

namespace Web.Controllers
{
    [SessionTimeout]
    public class CustomersController : BaseController
    {
        private readonly ICustomerClient _customerClient;
        private readonly ICompanyClient _companyClient;
        private readonly IConfig _config;
        public CustomersController(ICustomerClient customerClient,
                                   ICompanyClient companyClient,
                                   IConfig config)
        {
            _customerClient = customerClient;
            _companyClient = companyClient;
            _config = config;
        }
        // GET: Customers
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];

            HttpResponseMessage result = await _customerClient.GetList();
            List<CustomerInfo> customerInfos = null;

            if (result.IsSuccessStatusCode)
            {
                customerInfos = await result.Content.ReadAsAsync<List<CustomerInfo>>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(customerInfos);
        }

        // GET: Customers/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];
            return await getSingleInfo(id);
        }
        private async Task<ActionResult> getSingleInfo(int id)
        {
            HttpResponseMessage result = await _customerClient.Get(id);

            CustomerDto customer = null;

            if (result.IsSuccessStatusCode)
            {
                customer = await result.Content.ReadAsAsync<CustomerDto>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            CustomerInfo customerInfo = await GetCustomerInfo(customer);
            return View(customerInfo);
        }

        private async Task<ActionResult> getSingle(int id)
        {
            HttpResponseMessage result = await _customerClient.Get(id);

            CustomerDto customerDto = null;

            if (result.IsSuccessStatusCode)
            {
                customerDto = await result.Content.ReadAsAsync<CustomerDto>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }
            Customer customer = await GetCustomer(customerDto);
            return View(customer);
        }
        private async Task<Customer> GetCustomer(CustomerDto customerDto)
        {
            Customer customer = new Customer();
            customer.CustomerId = customerDto.CustomerId;
            customer.CustomerName = customerDto.CustomerName;
            customer.EmailId = customerDto.EmailId;
            customer.GSTNumber = customerDto.GSTNumber;
            customer.PAN = customerDto.PAN;
            customer.PrimaryMobileNumber = Convert.ToString(customerDto.PrimaryMobileNumber);
            customer.SecondaryMobileNumber = Convert.ToString(customerDto.SecondaryMobileNumber);

            List<Address> addresses = new List<Address>();
            Address address = new Address();
            address.AddressId = customerDto.Addresses[0].AddressId;
            address.City = customerDto.Addresses[0].City;
            address.DistrictId = customerDto.Addresses[0].DistrictId;
            address.FullAddress = customerDto.Addresses[0].FullAddress;
            address.Pincode = customerDto.Addresses[0].Pincode;
            address.StateId = customerDto.Addresses[0].StateId;
            addresses.Add(address);

            customer.Addresses = addresses;
            HttpResponseMessage result = await _companyClient.Get(customerDto.CompanyId);

            Company company = null;

            if (result.IsSuccessStatusCode)
            {
                company = await result.Content.ReadAsAsync<Company>();
            }

            customer.CompanyId = company.CompanyId;

            return customer;
        }
        private async Task<CustomerInfo> GetCustomerInfo(CustomerDto customer)
        {
            CustomerInfo customerInfo = new CustomerInfo();
            customerInfo.CustomerId = customer.CustomerId;
            customerInfo.CustomerName = customer.CustomerName;
            customerInfo.EmailId = customer.EmailId;
            customerInfo.GSTNumber = customer.GSTNumber;
            customerInfo.PAN = customer.PAN;
            customerInfo.PrimaryMobileNumber = Convert.ToString(customer.PrimaryMobileNumber);
            customerInfo.SecondaryMobileNumber = Convert.ToString(customer.SecondaryMobileNumber);
            customerInfo.Address = customer.Addresses[0].FullAddress;
            HttpResponseMessage result = await _companyClient.Get(customer.CompanyId);

            Company company = null;

            if (result.IsSuccessStatusCode)
            {
                company = await result.Content.ReadAsAsync<Company>();
            }
           
            customerInfo.CompanyName = company.CompanyName;

            return customerInfo;
        }

        // GET: Customers/Create
        public async Task<ActionResult> Create()
        {
            await BindDropDown();
            return View();
        }

        private async Task BindDropDown()
        {
            List<Company> companies = new List<Company>();
            HttpResponseMessage result = await _companyClient.GetList();
            if (result.IsSuccessStatusCode)
            {
                companies = await result.Content.ReadAsAsync<List<Company>>();
            }
            ViewData["Companies"] = companies.Select(x => new SelectListItem { Value = x.CompanyId.ToString(), Text = x.CompanyName });
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Customer collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var customerDto = new CustomerDto();
                    customerDto.CustomerName = collection.CustomerName;
                    customerDto.PrimaryMobileNumber = Convert.ToInt64(collection.PrimaryMobileNumber);
                    if (string.IsNullOrWhiteSpace(collection.SecondaryMobileNumber))
                    {
                        customerDto.SecondaryMobileNumber = null;
                    }
                    else
                    {
                        customerDto.SecondaryMobileNumber = Convert.ToInt64(collection.SecondaryMobileNumber);
                    }
                    customerDto.EmailId = collection.EmailId;
                    customerDto.PAN = collection.PAN;
                    customerDto.GSTNumber = collection.GSTNumber;
                    customerDto.CompanyId = collection.CompanyId;

                    List<AddressDto> addressDtos = new List<AddressDto>();
                    AddressDto addressDto = new AddressDto();
                    addressDto.FullAddress = collection.Addresses[0].FullAddress;
                    addressDto.DistrictId = 1;
                    addressDto.StateId = 1;
                    addressDtos.Add(addressDto);
                    customerDto.Addresses = addressDtos;

                    HttpResponseMessage result = await _customerClient.Add(customerDto);

                    return await checkResult(result, _config.getCreatedInnerMessage());
                }
                else
                {
                    ViewBag.Message = _config.errorMessage();
                    ViewBag.InnerMessage = ModelState.Values.Select(v => v.Errors).FirstOrDefault();
                    await BindDropDown();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = ex.Message;
                await BindDropDown();
                return View();
            }            
        }

        private async Task<ActionResult> checkResult(HttpResponseMessage result, string innerMessage)
        {
            if (result == null)
            {
                return RedirectToAction("Timeout", "Login");
            }

            var contents = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                Customer customer = JsonConvert.DeserializeObject<Customer>(contents);
                TempData["Message"] = _config.successMessage();
                TempData["InnerMessage"] = innerMessage;
                return RedirectToAction("Details", new { id = customer.CustomerId });
            }
            else
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = contents;
                await BindDropDown();
                return View();
            }
        }
        // GET: Customers/Edit/5
        public async Task<ActionResult> Edit(int id)
        {            
            await BindDropDown();
            return await getSingle(id);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Customer collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var customerDto = new CustomerDto();
                    customerDto.CustomerId = collection.CustomerId;
                    customerDto.CustomerName = collection.CustomerName;
                    customerDto.PrimaryMobileNumber = Convert.ToInt64(collection.PrimaryMobileNumber);
                    if (string.IsNullOrWhiteSpace(collection.SecondaryMobileNumber))
                    {
                        customerDto.SecondaryMobileNumber = null;
                    }
                    else
                    {
                        customerDto.SecondaryMobileNumber = Convert.ToInt64(collection.SecondaryMobileNumber);
                    }
                    customerDto.PAN = collection.PAN;
                    customerDto.GSTNumber = collection.GSTNumber;
                    customerDto.EmailId = collection.EmailId;
                    customerDto.CompanyId = collection.CompanyId;

                    List<AddressDto> addressDtos = new List<AddressDto>();
                    AddressDto addressDto = new AddressDto();
                    addressDto.FullAddress = collection.Addresses[0].FullAddress;
                    addressDto.DistrictId = 1;
                    addressDto.StateId = 1;
                    addressDtos.Add(addressDto);
                    customerDto.Addresses = addressDtos;

                    HttpResponseMessage result = await _customerClient.Edit(id, customerDto);
                    return await checkResult(result, _config.getUpdatedInnerMessage());
                }
                else
                {
                    ViewBag.Message = _config.errorMessage();
                    ViewBag.InnerMessage = ModelState.Values.Select(v => v.Errors).FirstOrDefault();
                    await BindDropDown();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = ex.Message;
                await BindDropDown();
                return View();
            }            
        }

        // GET: Customers/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            HttpResponseMessage result = await _customerClient.Get(id);
            if (!(result.IsSuccessStatusCode))
            {
                return RedirectToAction("Timeout", "Login");
            }
            return RedirectToAction("Details", "Customers", new { id = id });           
        }

        // POST: Customers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Customer collection)
        {
            try
            {
                HttpResponseMessage result = await _customerClient.Delete(id);
                return await checkResult(result, _config.getDeletedInnerMessage());
            }
            catch (Exception ex)
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = ex.Message;
                await BindDropDown();
                return View();
            }            
        }
    }
}