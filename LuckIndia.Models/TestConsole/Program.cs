using LuckIndia.APIs.DTO;
using LuckIndia.Services.RegistrationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var acctDto = new AccountDto
            {
                AccountTypeID = 3,
                CardNumber = 12345,
                ParentAccountID = 5,
                Type = new AccountTypeDto
                {
                    Id = 3

                },
                Password = "p@88w0rD",
                UserName = "9158716767"
            };

            UserDto dto = new UserDto
            {
                FirstName = "Mukti",
                MiddleName = "Atul",
                LastName = "Bhagwat",
                PhoeNumber = 9158716767,
                Address = "Add1",
            };
            var acctdtolist = new List<AccountDto>();
            acctdtolist.Add(acctDto);
            dto.accounts = acctdtolist;       

            Registration registration = new Registration();

            registration.Create(dto);
            //string status;
            //if (registration.SignIn("12345", "p@77w0rDss", out status))
            //{
            //    Console.WriteLine(status);
            //}
            //Console.WriteLine(status);


        }
    }
}
