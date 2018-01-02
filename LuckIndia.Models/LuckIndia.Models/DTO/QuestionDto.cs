using LuckIndia.Models;
using LuckIndia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.DTO
{
    public class QuestionDto : IModelDTO
    {
        public int? Id { get; set; }
        public String Statement { get; set; }

        public ICollection<OptionDto> Options { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}