using System;
using System.Collections.Generic;

namespace LuckIndia.APIs.DataInjectors

{
    public class UtcNowInjector : IDataInjectable
    {
        private const string UtcNowFunction = "utcnow()";
        private readonly List<Type> _types;

        public UtcNowInjector()
        {
            _types = new List<Type> { typeof(DateTime), typeof(DateTime?) };
        }

        public bool TryParseOccurences(string input, out string output)
        {
            output = default(string);

            try
            {
                if (!input.Contains(UtcNowFunction))
                {
                    return false;
                }

                var now = DateTime.UtcNow.ToString("o");

                output = input.Replace(UtcNowFunction, now);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryParseObject(Type type, string input, out object output)
        {
            output = null;

            try
            {
                if (input != UtcNowFunction || !_types.Contains(type))
                {
                    return false;
                }

                output = DateTime.UtcNow;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
