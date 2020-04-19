using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class HttpClientExtensions
    {
        public static HttpClient AddTokenToHeader(this HttpClient httpClient)
        {            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Convert.ToString(System.Web.HttpContext.Current.Session["accessToken"]));            
            return httpClient;
        }
    }
}
