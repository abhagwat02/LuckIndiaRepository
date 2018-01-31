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


        public void CrateDailyQuiz()
        {
            var allQuestions = GetAllQuestions().Result.ToList();
            var orderedList = allQuestions.OrderBy(x => x.Id).ToList();
            var lastUpdatedItem = orderedList.Where(x => x.Last == true).ToList();
            if(lastUpdatedItem.Count() != 0)
            {

            }
            else
            {
                var finalList = GetQuestionsInCircularWay(orderedList, orderedList.IndexOf(lastUpdatedItem.FirstOrDefault()));

            }
            int nStartTime = 8;


        }

        public IEnumerable<QuestionDto> GetQuestionsInCircularWay(IList<QuestionDto> fullList, int nIndex)
        {
            List<QuestionDto> list = fullList.Skip(nIndex).ToList();
            int index = 0;

            while (true)
                yield return list[index++ % list.Count];
        }



    }
}
