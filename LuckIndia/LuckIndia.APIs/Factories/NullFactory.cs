using LuckIndia.Models;
using LuckIndia.Models.Interfaces;
using System;


namespace LuckIndia.APIs.Factories

{
    public class NullFactory<TModel, TDTO> : Factory<TModel, TDTO>
        where TModel : Model
        where TDTO : IDTO
    {

        public override TModel FromDTO(TDTO dto)
        {
            throw new NotImplementedException();
        }

        public override TDTO ToDTO(TModel model)
        {
            throw new NotImplementedException();
        }
    }
}