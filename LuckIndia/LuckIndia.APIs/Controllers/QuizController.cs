using LuckIndia.Models;
using LuckIndia.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LuckIndia.APIs.Controllers
{
    public class QuizController : RestApiController<Quiz, QuizDto>
    {
        public override IHttpActionResult Post(QuizDto postedDto)
        {
            postedDto.DateCreated = DateTime.Now;
            postedDto.DateModified = DateTime.Now;
            return base.Post(postedDto);
        }
    }
}
