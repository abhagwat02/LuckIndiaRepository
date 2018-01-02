using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alphaeon.Services.EnterpriseAPI.DAL.CachingServices;
using Alphaeon.Services.EnterpriseAPI.DAL.CollectionFilters;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL
{
    internal sealed class Config
    {
        public static void Init()
        {
            var types = ModelClassCachingService.GetCurrent().GetModelTypes();

            foreach (var modelType in types)
            {
                FindAllSubclassForModelType(modelType, typeof(RoleCollectionFilter<>), RoleCollectionFilters);
                FindAllSubclassForModelType(modelType, typeof(RoleTypeCollectionFilter<>), RoleTypeCollectionFilters);
            }
        }

        private static void FindAllSubclassForModelType(Type modelType, Type baseType, IDictionary<Type, IEnumerable<object>> dictionaryToFill)
        {
            var objects = GetObjectsForModelType(modelType, baseType).ToList();

            if (objects.Count > 0)
            {
                dictionaryToFill.Add(modelType, objects);
            }
        }

        public static IEnumerable<object> GetObjectsForModelType(Type modelType, Type baseType)
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(x =>
                !x.IsAbstract &&
                x.BaseType != null &&
                x.BaseType.IsGenericType &&
                x.BaseType.GetGenericTypeDefinition() == baseType &&
                x.BaseType.GenericTypeArguments.First() == modelType)
                .Select(Activator.CreateInstance).ToList();
        }



        #region CollectionFilters

        private static readonly Dictionary<Type, IEnumerable<object>> RoleCollectionFilters = new Dictionary<Type, IEnumerable<object>>();
        private static readonly Dictionary<Type, IEnumerable<object>> RoleTypeCollectionFilters = new Dictionary<Type, IEnumerable<object>>();

        /// <summary>
        /// Gets all the RoleCollectionFilters for the given Model Type.
        /// </summary>
        /// <typeparam name="T">Type of Model</typeparam>
        /// <param name="roleIds">Valid RoleIds to retrieve since it's dependant on the user's roles.</param>
        public static IEnumerable<RoleCollectionFilter<T>> GetRoleCollectionFilters<T>(IEnumerable<int> roleIds) where T : Model
        {
            var key = typeof(T);
            return RoleCollectionFilters.ContainsKey(key)
                ? RoleCollectionFilters[key].Cast<RoleCollectionFilter<T>>()
                    .Where(x => roleIds.Contains(x.RoleId))
                : new List<RoleCollectionFilter<T>>().AsEnumerable();
        }

        /// <summary>
        /// Gets all the RoleTypeCollectionFilters for the given Model Type.
        /// </summary>
        /// <typeparam name="T">Type of Model</typeparam>
        /// <param name="roleTypeIds">Valid RoleTypeIds to retrieve since it's dependant on the user's roles.</param>
        public static IEnumerable<RoleTypeCollectionFilter<T>> GetRoleTypeCollectionFilters<T>(IEnumerable<int> roleTypeIds) where T : Model
        {
            var key = typeof(T);
            return RoleTypeCollectionFilters.ContainsKey(key)
                ? RoleTypeCollectionFilters[key].Cast<RoleTypeCollectionFilter<T>>()
                    .Where(x => roleTypeIds.Contains(x.RoleTypeId))
                : new List<RoleTypeCollectionFilter<T>>().AsEnumerable();
        }

        #endregion
    }
}
