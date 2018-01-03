using LuckIndia.Models;
using LuckIndia.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.Factories
{
    public class QuizFactory : Factory<Quiz, QuizDto>
    {
        public override Quiz FromDTO(QuizDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            var model = new Quiz
            {
                DateCreated = dto.DateCreated,
                DateModified = dto.DateModified,
                EndTime = dto.EndTime,
                QuizName = dto.QuizName,
                StartTime = dto.StartTime,
                Questions = dto.Questions.Select(x=> new QuestionFactory().FromDTO(x)).ToList()
                
                
            };
            return model;
        }

        public override QuizDto ToDTO(Quiz model)
        {
            if (model == null)
            {
                return null;
            }
            var dto = new QuizDto
            {
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                EndTime = model.EndTime,
                QuizName = model.QuizName,
                StartTime = model.StartTime,
                Questions = model.Questions.Select(x => new QuestionFactory().ToDTO(x)).ToList()
            };
            return dto;
        }
    }
}