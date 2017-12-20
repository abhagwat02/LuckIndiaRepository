using LuckIndia.APIs.DTO;
using LuckIndia.APIs.Enums;
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
    public class UsersController : BaseApiController
    {
        #region Ninject
        //  LuckIndiaRepository _repo;
        //public UsersController(LuckIndiaRepository repo)
        //{
        //    _repo = repo;
        //    _modelFectory = new ModelFactory();
        //}
        #endregion
        /********************************* Get ******************************/
        // GET api/Users
        public IEnumerable<UserDto>Get(bool bIncludeAccounts = true)
        {
           var users =  TheRepository.GetAllUsers(bIncludeAccounts).Select(u=> TheModelFactory.Create(u)).ToList();
           return users;
        }
        public UserDto Get(int Id, bool bIncludeAccounts = true)
        {
            var user = TheRepository.GetUser(Id, bIncludeAccounts);
            return TheModelFactory.Create(user);
        }

        /*********************************Post**************************************/

        // POST api/User
        public HttpResponseMessage Post([FromBody]UserDto user)
        {
            try
            {
                if (user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid User JSON");

                var newUser = TheModelFactory.Parse(user) as LuckUser;

                if (newUser == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read User");

                //Create User
               // TheRepository.CreateUser(newUser);
                var accts = user.accounts.Select(x => TheModelFactory.Parse(x)).ToList();
                accts.ForEach(x => x.user = newUser);

                //Create Account
                accts.ForEach(x => TheRepository.CreateAccount(x));
                newUser.accounts = accts;
                return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(newUser));

            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

            }
        }


    }
}
