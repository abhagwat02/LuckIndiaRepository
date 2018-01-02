using LuckIndia.Models.Interfaces;
using System;

namespace LuckIndia.DataModel.DAL.ValidationServices.StringValidationServices
{
    abstract class StringValidator : IValidatable
    {
        protected string Data { get; private set; }

        protected string PropertyName { get; private set; }

        protected StringValidator(String validateMe, string propertyName)
        {
            this.Data = validateMe;
            this.PropertyName = propertyName;
        }

        public abstract void Validate();
    }
}
