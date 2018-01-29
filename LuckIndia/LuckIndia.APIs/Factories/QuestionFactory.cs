using LuckIndia.Models;
using LuckIndia.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.Factories
{
    public class QuestionFactory : Factory<Question, QuestionDto>
    {
        public override Question FromDTO(QuestionDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            var model =  new Question
            {
                DateCreated = dto.DateCreated,
                DateModified = dto.DateModified,
                Last = dto.Last,
                Options = dto.Options.Select(x => new OptionFactory().FromDTO(x)).ToList(),
                Statement = dto.Statement
            };
            return model;
        }

        public override QuestionDto ToDTO(Question model)
        {
            if (model == null)
            {
                return null;
            }
            var dto =  new QuestionDto
            {
                Id = model.Id,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                Statement = model.Statement,
                Last = model.Last,
                Options = model.Options.Select(x => new OptionFactory().ToDTO(x)).ToList(),

            };
            return dto;
        }
    }
}