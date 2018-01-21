using System.Collections.Generic;
using System.Linq;
using LuckIndia.Models;
using LuckIndia.Models.Interfaces;

namespace LuckIndia.APIs.Factories
{
    public abstract class Factory<TModel, TDTO>
        where TModel : Model
        where TDTO : IDTO
    {
        public abstract TModel FromDTO(TDTO dto);

        public abstract TDTO ToDTO(TModel model);

        /// <summary>
        /// Converts a queryable of models to a collection of dtos.
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public IEnumerable<TDTO> ToDTOs(IEnumerable<TModel> models)
        {
            return models.Select(this.ToDTO);
        }
    }
}