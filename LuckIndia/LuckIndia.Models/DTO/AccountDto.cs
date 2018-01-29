using LuckIndia.Models.Attributes;
using LuckIndia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.Models.DTO
{
    [Include]
    public class AccountDto : IModelDTO
    {
        public int? Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int CardNumber { get; set; }
        public DateTime DateModified { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public int ParentAccountID { get; set; }
        public AccountTypeDto Type { get; set; }
        public int LuckUserID { get; set; }
        public int? AccountTypeID { get; set; }
        [Include]
        public UserDto user { get; set; }

    }
}