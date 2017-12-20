using LuckIndia.APIs.DTO;
using LuckIndia.APIs.Models;
using LuckIndia.DataModel;
using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LuckIndia.APIs.Controllers
{
    public class AccountsController : BaseApiController
    {
        //[Route("api/Users/{userId}/accounts/{Id}")]
        //[HttpGet]
        public IEnumerable<AccountDto> Get(int userId)
        {
            var accounts = TheRepository.GetAccountsForUser(userId)
                .ToList()
                .Select(a => TheModelFactory.Create(a));
            return accounts;
        }

        public AccountDto Get(int userId, int Id)
        {
           
            var account = TheRepository.GetAccount(Id);
                          
            if(account.user.Id == userId)
            {
                return TheModelFactory.Create(account);
            }
                
            return null;
        }
    }
}
