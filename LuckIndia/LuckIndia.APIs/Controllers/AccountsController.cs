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
    public class AccountsController : RestApiController<Account, AccountDto>
    {
        public override IHttpActionResult Post(AccountDto postedDto)
        {
            postedDto.DateCreated = DateTime.Now;
            postedDto.DateModified = DateTime.Now;
            return base.Post(postedDto);
        }
    }
}
