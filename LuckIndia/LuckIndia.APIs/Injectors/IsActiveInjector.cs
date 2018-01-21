using System;
using System.Text.RegularExpressions;

namespace LuckIndia.APIs.DataInjectors

{
    public class IsActiveInjector : IDataInjectable
    {
        private readonly Regex _regex = new Regex(@"isactive\((?<key>[A-z]?)\)");

        public bool TryParseOccurences(string input, out string output)
        {
            output = default(string);

            var valid = false;

            var now = DateTime.UtcNow.ToString("o");

            try
            {
                foreach (Match match in _regex.Matches(input))
                {
                    var key = match.Groups["key"].Value;
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        key += "/";
                    }

                    var swap =
                        string.Format(
                            "({0}StartDate lt DateTime'{1}' and ({0}EndDate eq null or {0}EndDate gt DateTime'{1}'))",
                            key,
                            now);

                    output = input.Replace(match.Value, swap);

                    valid = true;
                }

                return valid;
            }
            catch
            {
                return false;
            }
        }

        public bool TryParseObject(Type type, string input, out object output)
        {
            throw new NotImplementedException();
        }
    }
}