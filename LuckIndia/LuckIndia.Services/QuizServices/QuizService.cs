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

        public async Task<QuizDto> CreateQuiz(QuizDto dto)
        {
            try
            {
                HttpResponseMessage response;
                response = await _client.PostAsJsonAsync("quiz", dto).ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
                var retVal = response.Content.ReadAsAsync<QuizDto>().Result;
                return retVal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<QuestionQuizMapDto> CreateQuestionQuizMap(QuestionQuizMapDto dto)
        {
            try
            {
                HttpResponseMessage response;
                response = await _client.PostAsJsonAsync("QuestionQuizMap", dto).ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
                var retVal = response.Content.ReadAsAsync<QuestionQuizMapDto>().Result;
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
            if(lastUpdatedItem.Count() == 0)
            {
                 CreateQuizInDB(orderedList);
            }
            else
            {
                var finalList = GetQuestionsInCircularWay(orderedList, orderedList.IndexOf(lastUpdatedItem.FirstOrDefault()));
                 CreateQuizInDB(finalList.ToList());

            }
           // return true;
        }

        public async Task<QuizTypeDto[]> GetQuizTypes()
        {
            try
            {
                HttpResponseMessage response;
                // New code:
                response = await _client.GetAsync("QuizType", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                var retVal = response.Content.ReadAsAsync<QuizTypeDto[]>().Result;

                return retVal;
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public IEnumerable<QuestionDto> GetQuestionsInCircularWay(IList<QuestionDto> fullList, int nIndex)
        {
            List<QuestionDto> list = fullList.Skip(nIndex).ToList();
            int index = 0;

            while (true)
                yield return list[index++ % list.Count];
        }

        public void CreateQuizInDB(List<QuestionDto> fullList)
        {
            int nStartHour = 8;
            int nQues = 0;
            for (int quiz = 0; quiz <= 11; quiz++)
            {
                DateTime today = DateTime.Now;
                DateTime startTime = new DateTime(today.Year, today.Month, today.Day, nStartHour, 0, 0);
                DateTime endTime = new DateTime(today.Year, today.Month, today.Day, ++nStartHour, 0, 0);

                if (fullList.Count - nQues >= 3)

                {
                        var quiz_1 =  CreateQuiz(new QuizDto
                    {
                        StartTime = startTime,
                        EndTime = endTime,
                        QuizTypeId = 1,

                    }).Result;

                    var quiz_2 =  CreateQuiz(new QuizDto
                    {
                        StartTime = startTime,
                        EndTime = endTime,
                        QuizTypeId = 2,

                    }).Result;

                    var quiz_3 =  CreateQuiz(new QuizDto
                    {
                        StartTime = startTime,
                        EndTime = endTime,
                        QuizTypeId = 3,

                    }).Result;

                    var a = CreateQuestionQuizMap(
                    new QuestionQuizMapDto
                    {
                        QuestionId = fullList[nQues].Id.Value,
                        QuizId = quiz_1.Id.Value

                    }).Result;

                    var b = CreateQuestionQuizMap(
                      new QuestionQuizMapDto
                      {
                          QuestionId = fullList[++nQues].Id.Value,
                          QuizId = quiz_2.Id.Value

                      }).Result;

                    var c = CreateQuestionQuizMap(
                      new QuestionQuizMapDto
                      {
                          QuestionId = fullList[++nQues].Id.Value,
                          QuizId = quiz_3.Id.Value

                      }).Result;
                }

            }
            
        }

    }
}
