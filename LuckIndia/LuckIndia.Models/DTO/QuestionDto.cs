using LuckIndia.Models;
using LuckIndia.Models.Attributes;
using LuckIndia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.Models.DTO
{
    [Include]
    public class QuestionDto : IModelDTO
    {
        public QuestionDto()
        {
            Options =  new HashSet<OptionDto>();
        }
        public int? Id { get; set; }
        public String Statement { get; set; }
        public bool Last { get; set; }

        [Include]
        public ICollection<OptionDto> Options { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}