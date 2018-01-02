using System;

namespace LuckIndia.APIs.DataInjectors
{
    public interface IDataInjectable
    {
        bool TryParseOccurences(string input, out string output);

        bool TryParseObject(Type type, string input, out object output);
    }
}
