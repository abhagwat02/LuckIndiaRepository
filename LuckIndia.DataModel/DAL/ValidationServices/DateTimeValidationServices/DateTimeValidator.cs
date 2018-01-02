using System;
using Alphaeon.Services.EnterpriseAPI.Interfaces;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.DateTimeValidationServices
{
    //Date time's are trickey because of datetime vs datetime? 
    //I'm holding off on anymore work on date time validation until I have better thoughts on dealing with it.
    abstract class DateTimeValidator : IValidatable
    {
        protected readonly DateTime _data;
        protected readonly string _invalidMessage;

        public DateTimeValidator(DateTime date, string invalidMessage)
        {
            _data = date;
            _invalidMessage = invalidMessage;
        }

        public abstract void Validate();
    }
}