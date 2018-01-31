using LuckIndia.Services.QuizServices;
using LuckIndia.Services.RegistrationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var acctDto = new AccountDto
            //{
            //    AccountTypeID = 3,
            //    CardNumber = 12345,
            //    ParentAccountID = 5,
            //    Type = new AccountTypeDto
            //    {
            //        Id = 3

            //    },
            //    Password = "p@88w0rD",
            //    UserName = "9158716767"
            //};

            //UserDto dto = new UserDto
            //{
            //    Id = 4,
            //    FirstName = "Indra New Updated",
            //    MiddleName = "A",
            //    LastName = "Saha",
            //    PhoeNumber = 9158716767,
            //    Address = "Add1",

            //};

            //var acctDto = new AccountDto
            //{
                
            //    CardNumber = 12345,
            //    Type = new AccountTypeDto
            //    { 
            //        Id = 3,
            //        TypeName = "Dealer"
            //    },
            //    Password = "p@99w0rD",
            //    UserName = "8007926868"
            //};


            //var acctdtolist = new List<AccountDto>();
            //acctdtolist.Add(acctDto);
            //dto.accounts = acctdtolist;

            Registration registration = new Registration();

            //User create
            //HttpResponseMessage res =  registration.CrateLuckyUser(dto).Result;
            //var returned =  res.Content.ReadAsAsync<UserDto>().Result;

            //Get Login
            //string status;
            //if (registration.SignIn("12345", "p@77w0rDss", out status) != null)
            //{
            //    Console.WriteLine(status);
            //}
            //Console.WriteLine(status);

            //Get Users
            // var users = registration.GetAllLuckyUser().Result;

            //var option = new List<OptionDto>
            //{
            //    new OptionDto
            //    {
            //        Content = "Option 1"
            //    },
            //    new OptionDto
            //    {
            //        Content = "Option 2"
            //    },
            //    new OptionDto
            //    {
            //        Content = "Option 3"
            //    },
            //    new OptionDto
            //    {//
            //        Content = "Option 4"
            //    },
            //};
            //var ques = new QuestionDto
            //{
            //    Statement = "Question4",
            //    Options = option
            //};

            QuizService quizService = new QuizService();
            quizService.CrateDailyQuiz();
            //// var questions = quizService.GetAllQuestions().Result;
            //var Ques = quizService.CrateQuestion(ques).Result;

            // var bid = new BidsDto

            // var user = registration.UpdateLuckyUser(4,dto).Result;
           // var account = registration.UpdateUserAccount(5, acctDto);
           // Console.Write(JsonConvert.SerializeObject(account));

            

            

        }
    }
}
