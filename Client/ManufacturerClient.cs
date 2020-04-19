using Dtos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IManufacturerClient
    {
        Task<dynamic> GetList();
        Task<dynamic> Get(int id);
        Task<dynamic> Add(ManufacturerDto dto);
        Task<dynamic> Edit(int id, ManufacturerDto dto);
        Task<dynamic> Delete(int id);
    }
    public class ManufacturerClient : IManufacturerClient
    {        
        private static HttpClient _httpClient;
        private string BaseUriTemplate = ConfigurationManager.AppSettings["uri"] + "api/manufacturers";
        public ManufacturerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.AddTokenToHeader();
        }       

        public async Task<dynamic> Add(ManufacturerDto dto)
        {            
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseUriTemplate, dto);            
            return response;
        }

        public async Task<dynamic> Delete(int id)
        {            
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUriTemplate}/{id}");            
            return response;
        }

        public async Task<dynamic> Edit(int id, ManufacturerDto dto)
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
    }
}
