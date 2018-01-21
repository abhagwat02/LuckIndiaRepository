using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LuckIndia.DataModel.DAL.CachingServices
{
    sealed class CachingServiceReflector
    {
        private readonly Type _baseType;

        public CachingServiceReflector(Type baseType)
        {
            _baseType = baseType;
        }

        public Dictionary<Type, IEnumerable<Type>> GetCache()
        {
            var cacheDictionary = new Dictionary<Type, IEnumerable<Type>>();

            var modelTypes = ModelClassCachingService.GetCurrent().GetModelTypes();

            foreach (var modelType in modelTypes)
            {
                var types = this.GetTypesWithGenericTypeDefinition(modelType);

                if (types.Count > 0)
                {
                    cacheDictionary.Add(modelType, types);
                }
            }

            return cacheDictionary;
        }

        private List<Type> GetTypesWithGenericTypeDefinition(Type modelType)
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(x =>
                !x.IsAbstract &&
                x.BaseType != null &&
                x.BaseType.IsGenericType &&
                x.BaseType.GetGenericTypeDefinition() == _baseType &&
                x.BaseType.GenericTypeArguments.First() == modelType).ToList();
        }
    }
}
