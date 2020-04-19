using ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Logger;

namespace Client
{
    public interface ILoginClient
    {
        Task<dynamic> Authenticate(Login login);
    }
    public class LoginClient : ILoginClient
    {
        private static HttpClient _httpClient;
        private string BaseUriTemplate = ConfigurationManager.AppSettings["uri"] + "token";     
        public LoginClient()
        {
            _httpClient = new HttpClient();            
        }        

        public async Task<dynamic> Authenticate(Login login)
        {            
            login.grant_type = "password";
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("grant_type", "password"));
            postData.Add(new KeyValuePair<string, string>("username", login.username));
            postData.Add(new KeyValuePair<string, string>("password", login.password));
            FormUrlEncodedContent content = new FormUrlEncodedContent(postData);
            HttpResponseMessage response = await _httpClient.PostAsync(BaseUriTemplate, content);            
            return response;
        }
    }
}
