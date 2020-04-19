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
    public interface IRegisterClient
    {
        Task<dynamic> Register(ResourceDto login);
    }
    public class RegisterClient: IRegisterClient
    {
        private static HttpClient _httpClient;
        private string BaseUriTemplate = ConfigurationManager.AppSettings["uri"] + "api/account/register";
        public RegisterClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.AddTokenToHeader();
        }
        public async Task<dynamic> Register(ResourceDto login)
        {
            //login.grant_type = "password";
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("role", login.RoleName));
            postData.Add(new KeyValuePair<string, string>("username", login.ResourceName));
            postData.Add(new KeyValuePair<string, string>("password", login.Password));
            postData.Add(new KeyValuePair<string, string>("confirmpassword", login.ConfirmPassword));
            postData.Add(new KeyValuePair<string, string>("email", login.Email));
            FormUrlEncodedContent content = new FormUrlEncodedContent(postData);
            HttpResponseMessage response = await _httpClient.PostAsync(BaseUriTemplate, content);
            return response;
        }
    }
}
