using System.Text.RegularExpressions;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.TagValidationServices
{
    sealed class TagValidator
    {
        private const string TAG_CHARS_INVALID = @"[\p{P}\p{S}-[`-]]";


        public string Sanitize(string tag)
        {
            var sanitizedTag = tag.Replace("'", "`").Replace("&", " and ").Replace("\"", "``"); // ' & "
            sanitizedTag = sanitizedTag.Replace("_", " ");
            sanitizedTag = Regex.Replace(sanitizedTag, @"\s+", " "); //Eliminate multiple spaces
            sanitizedTag = Regex.Replace(sanitizedTag, TAG_CHARS_INVALID, "");
            return sanitizedTag.Trim().ToUpper();
        }


        public bool IsValid(string tag)
        {
            var sanitizedTag = this.Sanitize(tag);
            return string.Compare(tag, sanitizedTag) == 0;
        }
    }
}