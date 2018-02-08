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
    public class QuizTypeController : RestApiController<QuizType, QuizTypeDto>
    {
        public override IHttpActionResult Post(QuizTypeDto postedDto)
        {
            postedDto.DateCreated = DateTime.Now;
            postedDto.DateModified = DateTime.Now;
            return base.Post(postedDto);
        }
    }
}
