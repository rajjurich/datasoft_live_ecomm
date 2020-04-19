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
    public interface IRoleClient
    {
        Task<dynamic> GetList();
        Task<dynamic> Get(int id);
    }
    public class RoleClient : IRoleClient
    {
        private static HttpClient _httpClient;
        private string BaseUriTemplate = ConfigurationManager.AppSettings["uri"] + "api/roles";
        public RoleClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.AddTokenToHeader();
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
