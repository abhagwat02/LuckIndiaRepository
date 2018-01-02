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
            var model =  new Option
            {
                Content = dto.Content,
                DateCreated = dto.DateCreated,
                DateModified = dto.DateModified,

            };
            return model;
        }

        public override OptionDto ToDTO(Option model)
        {
            var dto = new OptionDto
            {
                Content = model.Content,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                Id = model.Id,
                Question = new QuestionFactory().ToDTO(model.Question)
                

            };
            return dto;
        }
    }
}