using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.Models
{
    public class Option
    {
        public int Id { get; set; }
        public String Content { get; set; }

        public Question Question { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}