using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.Models
{
    public class Question
    {
        public int Id { get; set; }
        public String Statement { get; set; }

        public ICollection<Option> Options { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}