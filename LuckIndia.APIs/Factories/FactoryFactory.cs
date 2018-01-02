using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LuckIndia.Models;
using LuckIndia.Models.Interfaces;

namespace LuckIndia.APIs.Factories
{
    public class FactoryFactory
    {
        //Tuple<Model,IDTO> is what it is, but Types are needed because of reflection
        private static Dictionary<Tuple<Type, Type>, object> _factories;

        public static void Init()
        {
            _factories = new Dictionary<Tuple<Type, Type>, object>();

            var factories = Assembly.GetExecutingAssembly().GetTypes().Where(x =>
                x.BaseType != null &&
                x.BaseType.IsGenericType &&
                !x.ContainsGenericParameters &&
                x.BaseType.GetGenericTypeDefinition() == typeof(Factory<,>));

            foreach (var factory in factories)
            {
                // We know this can't be null becuase of the query above.
                // ReSharper disable once PossibleNullReferenceException
                var types = factory.BaseType.GenericTypeArguments;

                if (types.Length != 2 && types[0] != typeof(Model) && types[1] != typeof(IDTO))
                {
                    continue;
                }

                var key = Tuple.Create(types[0], types[1]);

                if (_factories.ContainsKey(key))
                {
                    throw new Exception(String.Format("Factory<{0},{1}> already exists.", key.Item1.Name, key.Item2.Name));
                }

                _factories.Add(key, Activator.CreateInstance(factory));
            }
        }

        public static Factory<TModel, TDTO> GetFactory<TModel, TDTO>()
            where TModel : Model
            where TDTO : IDTO
        {
            var key = Tuple.Create(typeof(TModel), typeof(TDTO));

            if (_factories.ContainsKey(key))
            {
                return (Factory<TModel, TDTO>)_factories[key];
            }

            return new NullFactory<TModel, TDTO>();
        }
    }
}
