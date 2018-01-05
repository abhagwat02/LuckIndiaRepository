using LuckIndia.APIs.DTO;
using LuckIndia.APIs.HttpExtension;
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

        private async Task<HttpResponseMessage> RetreiveAccount(string path)
        {
            try
            {
                HttpResponseMessage response;
                // New code:
                response = await _client.GetAsync(path,HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

                return response;
            }
            catch(Exception ex)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
            return null;
        }


        public async Task<UserDto> CrateLuckyUser(UserDto dto)
        {
            try
            {
                HttpResponseMessage response;
                response = await _client.PostAsJsonAsync("users", dto).ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
                var retVal = response.Content.ReadAsAsync<UserDto>().Result;

                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserDto> UpdateLuckyUser(int Id, UserDto dto)
        {
            try
            {
                HttpResponseMessage response;
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(dto));
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                response = await _client.PatchAsync(new Uri(BaseUri + "users\\" + Id), httpContent);//.ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
                var retVal = response.Content.ReadAsAsync<UserDto>().Result;
                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AccountDto> UpdateUserAccount(int Id, AccountDto dto)
        {
            try
            {
                HttpResponseMessage response;
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(dto));
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                response = await _client.PatchAsync(new Uri(BaseUri + "accounts\\" + Id), httpContent).ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
                var retVal = response.Content.ReadAsAsync<AccountDto>().Result;
                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserDto[]> GetAllLuckyUser()
        {
            try
            {
                HttpResponseMessage response;
                // New code:
                response = await _client.GetAsync("users");
                var retVal = response.Content.ReadAsAsync<UserDto[]>().Result;
                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return null;
        }


        public async Task<UserDto> GetLuckyUserByID(int Id)
        {
            try
            {
                HttpResponseMessage response;
                // New code:
                response = await _client.GetAsync("users\\"+Id);
                var retVal = response.Content.ReadAsAsync<UserDto>().Result;
                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<HttpResponseMessage> DeleteLuckyUser(int Id)
        {
            try
            {
                HttpResponseMessage response;
                // New code:
                response = await _client.DeleteAsync("users\\"+Id);
                var retVal = response.Content.ReadAsAsync<HttpResponseMessage>().Result;
                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return null;
        }



    }
}
