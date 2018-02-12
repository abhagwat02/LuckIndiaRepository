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
                StartTime = dto.StartTime,
                QuizTypeId = dto.QuizTypeId
                
                
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
                Id = model.Id,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                EndTime = model.EndTime,
                StartTime = model.StartTime,
                QuizTypeId = model.QuizTypeId,
                type = new QuizTypeFactory().ToDTO(model.type)
                
            };
            return dto;
        }
    }
}