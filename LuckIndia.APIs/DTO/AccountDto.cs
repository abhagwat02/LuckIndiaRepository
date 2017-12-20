using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.DTO
{
    public class AccountDto
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public int CardNumber { get; set; }
        public DateTime? DateModified { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public int ParentAccountID { get; set; }
        public AccountTypeDto Type { get; set; }

    }
}