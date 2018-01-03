using LuckIndia.APIs.DTO;
using LuckIndia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.Models.DTO
{
   public class QuizDto : IModelDTO
    {
        public QuizDto()
        {
            Questions  = new HashSet<QuestionDto>();
        }

        public int? Id { get; set; }
        public String QuizName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ICollection<QuestionDto> Questions { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
