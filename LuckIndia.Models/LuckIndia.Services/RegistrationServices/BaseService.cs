using LuckIndia.APIs.Models;
using LuckIndia.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.Services.RegistrationServices
{
    public class BaseService
    {
        public HttpClient _client { get; set; }
        public string BaseUri { get; set; }
        //public ModelFactory TheModelFactory { get; set; }

        public BaseService()
        {
            BaseUri = /*"http://luckindiaapi.azurewebsites.net/";*/ "http://localhost:7721/";
            _client = new HttpClient();
            _client.BaseAddress = new Uri(BaseUri);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "e897aa05df964d188472839559cfd080");
            //TheModelFactory = new ModelFactory(new HttpRequestMessage(), new LuckIndiaRepository(new LuckIndiaDBContext()));
        }


    }
}
