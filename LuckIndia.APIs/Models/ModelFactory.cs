using LuckIndia.APIs.DTO;
using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;

namespace LuckIndia.APIs.Models
{
    public class ModelFactory
    {

        public UrlHelper _urlHelper { get; set; }

        public ModelFactory(HttpRequestMessage request)
        {
            _urlHelper = new UrlHelper(request);
        }
        public UserDto Create(LuckUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Address = user.Address,
                LastName = user.LastName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                PhoeNumber = user.PhoeNumber,
                accounts = user.accounts.Select(a => Create(a)),
                Url = _urlHelper.Link("Users",new { Id = user.Id})
            };
        }

        public AccountDto Create(Account acct)
        {
            return new AccountDto
            {
                Id = acct.Id,
                CardNumber = acct.CardNumber,
                ParentAccountID = acct.ParentAccountID,
                Type = Create(acct.Type),
                Url = _urlHelper.Link("Account", new { userid = acct.user.Id, Id = acct.Id })


            };
        }

        public AccountTypeDto Create(AccountType acctType)
        {
            return new AccountTypeDto
            {
                Id = acctType.Id,
                TypeName = acctType.TypeName

            };
        }
    }

       
    }
