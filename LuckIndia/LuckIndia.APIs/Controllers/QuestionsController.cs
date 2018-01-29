using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using LuckIndia.Models.DTO;

namespace LuckIndia.APIs.Controllers
{
    public class QuestionsController : RestApiController<Question, QuestionDto>
    {
        public override IHttpActionResult Post(QuestionDto postedDto)
        {
            postedDto.DateCreated = DateTime.Now;
            postedDto.DateModified = DateTime.Now;
            return base.Post(postedDto);
        }
        public override IHttpActionResult Patch(int? id, JObject json)
        {
            return base.Patch(id, json);
        }
    }
}
