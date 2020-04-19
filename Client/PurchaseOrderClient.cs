using Dtos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IPurchaseOrderClient
    {
        Task<dynamic> GetList();
        Task<dynamic> Get(int id);
        Task<dynamic> GetName(int id);
        Task<dynamic> Add(PurchaseOrderDto dto);
        Task<dynamic> Edit(int id, PurchaseOrderDto dto);
        Task<dynamic> Paid(int id, PurchaseOrderDto dto);
        Task<dynamic> Delete(int id);
    }
    public class PurchaseOrderClient: IPurchaseOrderClient
    {
        private static HttpClient _httpClient;
        private string BaseUriTemplate = ConfigurationManager.AppSettings["uri"] + "api/PurchaseOrders";

        public PurchaseOrderClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.AddTokenToHeader();
        }

        public async Task<dynamic> Add(PurchaseOrderDto dto)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseUriTemplate, dto);
            return response;
        }

        public Task<dynamic> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> Edit(int id, PurchaseOrderDto dto)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseUriTemplate}/{id}", dto);
            return response;
        }

        public async Task<dynamic> Get(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUriTemplate}/{id}");
            return response;
        }

        public async Task<dynamic> GetList()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(BaseUriTemplate);
            return response;
        }

        public Task<dynamic> GetName(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> Paid(int id, PurchaseOrderDto dto)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseUriTemplate}/Pay/{id}", dto);
            return response;
        }
    }
}
