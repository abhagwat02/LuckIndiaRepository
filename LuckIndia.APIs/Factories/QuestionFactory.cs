using LuckIndia.APIs.DTO;
using LuckIndia.Models;
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
           var model =  new Question
            {
                DateCreated = dto.DateCreated,
                DateModified = dto.DateModified,
                Options = dto.Options.Select(x => new OptionFactory().FromDTO(x)).ToList(),
                Statement = dto.Statement
            };
            return model;
        }

        public override QuestionDto ToDTO(Question model)
        {
            var dto =  new QuestionDto
            {
                Id = model.Id,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                Statement = model.Statement,
                //Options = model.Options.Select(x => new OptionFactory().ToDTO(x)).ToList(),

            };
            return dto;
        }
    }
}