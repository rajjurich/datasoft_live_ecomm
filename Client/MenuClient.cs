using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Client
{
    public interface IMenuClient
    {
        Task<dynamic> GetList();
        Task<dynamic> GetAllList();
        Task<dynamic> Get(int id);
        Task<dynamic> Add(Menu dto);
        Task<dynamic> Edit(int id, Menu dto);
        Task<dynamic> Delete(int id);

    }
    public class MenuClient : IMenuClient
    {
        private static HttpClient _httpClient;
        private string BaseUriTemplate = ConfigurationManager.AppSettings["uri"] + "api/Menus";
        public MenuClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.AddTokenToHeader();
        }

        public async Task<dynamic> Add(Menu dto)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseUriTemplate, dto);
            return response;
        }

        public async Task<dynamic> Delete(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUriTemplate}/{id}");
            return response;
        }

        public async Task<dynamic> Edit(int id, Menu dto)
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

        public async Task<dynamic> GetAllList()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUriTemplate}/all");
            return response;
        }
    }
}
