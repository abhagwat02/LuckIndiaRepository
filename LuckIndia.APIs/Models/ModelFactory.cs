using LuckIndia.APIs.DTO;
using LuckIndia.DataModel;
using LuckIndia.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace LuckIndia.APIs.Models
{
    public class ModelFactory
    {

        public UrlHelper _urlHelper { get; set; }
        public LuckIndiaRepository _repo { get; set; }

        public ModelFactory(HttpRequestMessage request, LuckIndiaRepository repo)
        {
            _urlHelper = new UrlHelper(request);
            _repo = repo;
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
                UserName = acct.UserName,
                Password = acct.Password,
                Type = Create(acct.Type),
              //  Url = _urlHelper.Link("Account", new { userid = acct.user.Id, Id = acct.Id })

                
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


        internal LuckUser Parse(UserDto user)
        {
            //try
            //{
                return new LuckUser
                {
                    Address = user.Address,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Id = user.Id,
                    MiddleName = user.MiddleName,
                    PhoeNumber = user.PhoeNumber,
                };
           // }
            //catch (Exception ex)
            //{
            //    //return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            //}

        }

        internal Account Parse(AccountDto account)
        {
            //try
            //{
                return new Account
                {
                   CardNumber  = account.CardNumber,
                    DateModified = account.DateModified,
                    DateCreated = account.DateCreated,
                    ParentAccountID = account.ParentAccountID,
                    Password = account.Password,
                    UserName = account.UserName,
                   Type = Parse(account.Type)
                   
                };
            //}
            //catch (Exception ex)
            //{
            //    //return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            //}



        }

        internal AccountType Parse(AccountTypeDto accounttype)
        {
            //try
            //{
            return new AccountType
            {
                Id = accounttype.Id,
                TypeName = accounttype.TypeName
                
            };
            //}
            //catch (Exception ex)
            //{
            //    //return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            //}



        }
    }

       
    }
