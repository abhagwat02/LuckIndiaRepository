using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.Models
{
    public class LuckUser
    {
        public int Id { get; set; }
        public String FirstName { get; set; }
        public String MiddleName  { get; set; }
        public String LastName { get; set; }
        public long PhoeNumber { get; set; }
        public String Address { get; set; }
        public ICollection<Account> accounts { get; set; }

    }
}