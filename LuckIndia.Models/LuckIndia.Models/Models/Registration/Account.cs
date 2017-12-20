using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.Models
{
    public class Account
    {
        public int Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public int CardNumber { get; set; }
        public DateTime? DateModified { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public LuckUser user { get; set; }
        public AccountType Type { get; set; }
        public int ParentAccountID { get; set; }
    }
}