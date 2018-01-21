using LuckIndia.DataModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.DataModel.Authorizations
{
    public class User : IUser
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
    }
}
