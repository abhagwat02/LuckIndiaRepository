using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.CachingServices;
using LuckIndia.DataModel.DAL.CrudHelpers;
using LuckIndia.Models;
using LuckIndia.DataModel;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CrudHelpers
{
    sealed class CollectionHelper<T> : CrudHelper<T> where T : Model
    {
        public CollectionHelper(LuckIndiaDBContext context) : base(null, context) { }

        public IQueryable<T> GetCollection()
        {
            ////fire this so read permissions are checked.
            this.Execute();

            //var roleTypeCollection = this.GetCollectionForRoleTypes();
            var roleCollection = this.GetCollectionForRoles();

            //var roleUnionCollection = roleTypeCollection.Union(roleCollection);

            return roleCollection;/// this.ApplyApplicationCollectionFilters(roleUnionCollection);
            //return null;
        }

        private IQueryable<T> GetCollectionForRoles()
        {
            //VishalJoshi07931
           // var filteredCollection = this.Context.Set<T>().Take(0);
            var filteredCollection = this.Context.Set<T>().AsQueryable();
            //VishalJoshi07931

            //var userRoleIds = this.Context.GetCurrentUser().Roles.Select(x => x.Id);


            //var roleCollectionFilters = Config.GetRoleCollectionFilters<T>(userRoleIds);

            //filteredCollection = roleCollectionFilters.Aggregate(filteredCollection,
            //    (current, filter) => current.Union(filter.ApplyFilter(this.Context)));

            return filteredCollection;
        }

        private IQueryable<T> GetCollectionForRoleTypes()
        {
            ////VishalJoshi07931
            ////var filteredCollection = this.Context.Set<T>().Take(0);
            //var filteredCollection = this.Context.Set<T>().AsQueryable();
            ////VishalJoshi07931

            //var userroletypeids = this.context.getcurrentuser().roles.select(x => x.id);

            //var roletypecollectionfilters = config.getroletypecollectionfilters<t>(userroletypeids);

            //filteredcollection = roletypecollectionfilters.aggregate(filteredcollection,
            //    (current, filter) => current.union(filter.applyfilter(this.context)));

            //return filteredcollection;
            return null;
        }


        //private IQueryable<T> ApplyApplicationCollectionFilters(IQueryable<T> collection)
        //{
        //    var applicationId = this.Context.GetCurrentApplicationId();

        //    var collectionFilterValues = ApplicationCollectionFilterCachingService.GetCurrent()
        //        .GetCollectionFilterValuesForApplication(applicationId).ToList();

        //    if (!collectionFilterValues.Any())
        //    {
        //        //no collection filters of any type for this application, return the original.
        //        return collection;
        //    }

        //    var collectionFilters = CollectionFilterCachingService.GetCurrent()
        //        .GetCollectionFilters(collectionFilterValues, collection)
        //        .ToList();

        //    if (!collectionFilters.Any())
        //    {
        //        //no collection filters for this, return the original.
        //        return collection;
        //    }

        //    if (collectionFilters.Count() > 1)
        //    {
        //        //throw new InvalidDataException("Multiple collection filters found.");
        //    }

        //    var collectionFilter = collectionFilters.First();
        //    return collectionFilter.ApplyFilter();
        //}

        protected override void ValidateApplication()
        {
            //var applicationId = this.Context.GetCurrentApplicationId();

            //var valid = ApplicationPermissionCachingService.GetCurrent().CanRead(applicationId, this.ModelClassId);

            //if (!valid)
            //{
            //    throw new ForbiddenException("Application does not have permission. For model class id : " + this.ModelClassId , ExceptionSeverity.Information);
            //}
        }

        protected override void ValidateUser()
        {
            //var userId = this.Context.GetCurrentUser().Id;

            //var valid = RolePermissionCachingService.GetCurrent().CanRead(this.ModelClassId, Context);

            //if (!valid)
            //{
            //    throw new ForbiddenException("No permission. For model class id : " + this.ModelClassId, ExceptionSeverity.Information);
            //}
        }

        protected override void ValidateCollection()
        {
            //Nothing to do here because this is the collection
        }
    }
}
