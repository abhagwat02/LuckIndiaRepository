using LuckIndia.APIs.DTO;
using LuckIndia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.Services.RegistrationServices
{
    class test
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class Registration : BaseService
    {
        public bool SignIn(string username, string password, out string ErrorString)
        {
            var userAcct = RetreiveAccount("accounts?$filter=UserName eq\'" + username +"\'").Result;
            var dto = userAcct.Content.ReadAsAsync<AccountDto[]>().Result;
            if (dto.Length > 0)
            {
                ErrorString = "Found";
                return dto[0].Password.CompareTo(password) == 0;
            }
            else
            {
                ErrorString = "User not found";
                return false;
            }
        }

        public void Create(UserDto dto)
        {
            CrateLuckyUser(dto);
        }

        private async Task<HttpResponseMessage> RetreiveAccount(string path)
        {
            try
            {
                HttpResponseMessage response;
                // New code:
                response = await _client.GetAsync(path);
                return response;
            }
            catch(Exception ex)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
            return null;
        }


        public async Task<HttpResponseMessage> CrateLuckyUser(UserDto dto)
        {
            try
            {
                HttpResponseMessage response;
                response = await _client.PostAsJsonAsync("users", dto).ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
