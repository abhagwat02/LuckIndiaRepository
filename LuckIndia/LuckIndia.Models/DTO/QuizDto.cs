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


        public int? Id { get; set; }
        public DateTime StartTime { get; set; }
        public int QuizTypeId { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public QuizTypeDto type { get; set; }
    }
}
