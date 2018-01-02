using LuckIndia.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.Models
{
    [Include]
    public class AccountType : Model
    {
        public override int Id { get; set; }

        public String TypeName { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}