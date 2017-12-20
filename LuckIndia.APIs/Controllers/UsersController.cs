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
    public class UsersController : BaseApiController
    {
      //  LuckIndiaRepository _repo;
        //public UsersController(LuckIndiaRepository repo)
        //{
        //    _repo = repo;
        //    _modelFectory = new ModelFactory();
        //}

        /********************************* Get ******************************/
        // GET api/Users
        public IEnumerable<UserDto>Get(bool bIncludeAccounts = true)
        {
            
           var users =  TheRepository.GetAllUsers(bIncludeAccounts).Select(u=> TheModelFactory.Create(u)).ToList();
           return users;
        }
        public LuckUser Get(int Id, bool bIncludeAccounts = true)
        {
            var user = TheRepository.GetUser(Id, bIncludeAccounts);
            return user;
        }

        /*********************************Post**************************************/

        // POST api/User
        public void Post([FromBody]LuckUser user)
        {

            TheRepository.InsertUser(user);
        }


    }
}
