using LuckIndia.Models.Attributes;
using LuckIndia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.DTO
{
    [Include]
    public class OptionDto : IModelDTO
    {
        public int? Id { get; set; }
        public String Content { get; set; }
        public int QuestionID { get; set; }

        public QuestionDto Question { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}