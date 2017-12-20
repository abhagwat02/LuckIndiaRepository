using LuckIndia.APIs.Models;
using LuckIndia.DataModel;

using System.Web.Http;

namespace LuckIndia.APIs.Controllers
{
    public abstract class BaseApiController : ApiController
    {
         ModelFactory _modelFactory { get; set; }
         LuckIndiaRepository _repo { get; set; }
        protected ModelFactory TheModelFactory
        {
            get
            {
                _modelFactory = new ModelFactory(this.Request, TheRepository);
                return _modelFactory;

            }
        }

        protected LuckIndiaRepository TheRepository
        {
            get
            {
                _repo = new LuckIndiaRepository(new LuckIndiaDBContext());
                return _repo;

            }
        }

    }
}
