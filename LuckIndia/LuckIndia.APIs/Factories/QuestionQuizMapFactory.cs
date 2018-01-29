using LuckIndia.Models.DTO;
using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.Factories
{
    public class QuestionQuizMapFactory : Factory<QuestionQuizMap, QuestionQuizMapDto>
    {
        public override QuestionQuizMap FromDTO(QuestionQuizMapDto dto)
        {

            if (dto == null)
            {
                return null;
            }
            var model = new QuestionQuizMap
            {
                QuestionId = dto.QuestionId,
                QuizId = dto.QuizId,

            };
            return model;
            
            return null;
        }

        public override QuestionQuizMapDto ToDTO(QuestionQuizMap model)
        {
            if (model == null)
            {
                return null;
            }
            var dto = new QuestionQuizMapDto
            {
                Id = model.Id,
                QuestionId = model.QuestionId,
                QuizId = model.QuizId,
                question = new QuestionFactory().ToDTO(model.question),
                quiz = new QuizFactory().ToDTO(model.quiz),
                
            };
            return dto;
        }
    }
}