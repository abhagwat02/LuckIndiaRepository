using LuckIndia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.DTO
{
    public class AccountTypeDto : IModelDTO
    {

        public string Url { get; set; }
        public int? Id { get; set; }
        public String TypeName { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}