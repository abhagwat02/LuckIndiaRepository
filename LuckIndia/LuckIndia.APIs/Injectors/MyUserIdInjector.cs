using System;
using System.Collections.Generic;

namespace LuckIndia.APIs.DataInjectors

{
    public class MyUserIdInjector : IDataInjectable
    {
        private readonly int _userId;

        private const string MyUserIdFunction = "myuserid()";
        private readonly List<Type> _types;

        public MyUserIdInjector(int userId)
        {
            _userId = userId;
            _types = new List<Type> { typeof(int) };
        }

        public bool TryParseOccurences(string input, out string output)
        {
            output = default(string);

            try
            {
                if (!input.Contains(MyUserIdFunction))
                {
                    return false;
                }

                output = input.Replace(MyUserIdFunction, _userId.ToString());

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
                if (input != MyUserIdFunction || !_types.Contains(type))
                {
                    return false;
                }

                output = _userId;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasOccurences(string input)
        {
            return input.Contains(MyUserIdFunction);
        }
    }
}
