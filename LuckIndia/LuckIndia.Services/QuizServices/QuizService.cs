using LuckIndia.Models.DTO;
using LuckIndia.Services.RegistrationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.Services.QuizServices
{
    public class QuizService : BaseService
    {
        public async Task<QuestionDto> CrateQuestion(QuestionDto dto)
        {
            try
            {
                HttpResponseMessage response;
                response = await _client.PostAsJsonAsync("questions", dto).ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
                var retVal = response.Content.ReadAsAsync<QuestionDto>().Result;
                return retVal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<QuestionDto[]> GetAllQuestions()
        {
            try
            {
                HttpResponseMessage response;
                // New code:
                response = await _client.GetAsync("questions", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                var retVal = response.Content.ReadAsAsync<QuestionDto[]>().Result;

                return retVal;
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }


        public async Task<QuestionDto> CrateDailyQuiz()
        {
            var allQuestions = GetAllQuestions().Result.ToList();
            

            int nStartTime = 8;


            return null;
        }




    }
}
