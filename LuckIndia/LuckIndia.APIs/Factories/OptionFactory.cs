using LuckIndia.APIs.DTO;
using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.Factories
{
    public class OptionFactory : Factory<Option, OptionDto>
    {
        public override Option FromDTO(OptionDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            var model =  new Option
            {
                Content = dto.Content,
                QuestionID = dto.QuestionID,
                DateCreated = dto.DateCreated,
                DateModified = dto.DateModified,

            };
            return model;
        }

        public override OptionDto ToDTO(Option model)
        {
            if (model == null)
            {
                return null;
            }
            var dto = new OptionDto
            {
                Content = model.Content,
                QuestionID = model.QuestionID,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                Id = model.Id,
               // Question = new QuestionFactory().ToDTO(model.Question)
                

            };
            return dto;
        }
    }
}