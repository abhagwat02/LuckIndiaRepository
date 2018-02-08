using LuckIndia.Models;
using LuckIndia.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.Factories
{
    public class QuizTypeFactory : Factory<QuizType, QuizTypeDto>
    {
        public override QuizType FromDTO(QuizTypeDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            var model = new QuizType
            {
                name = dto.name,
                DateCreated = dto.DateCreated,
                DateModified = dto.DateModified,

            };
            return model;
        }

        public override QuizTypeDto ToDTO(QuizType model)
        {
            if (model == null)
            {
                return null;
            }
            var dto = new QuizTypeDto
            {
                Id = model.Id,
                name = model.name,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,

                // Question = new QuestionFactory().ToDTO(model.Question)


            };
            return dto;
        }
    }
}