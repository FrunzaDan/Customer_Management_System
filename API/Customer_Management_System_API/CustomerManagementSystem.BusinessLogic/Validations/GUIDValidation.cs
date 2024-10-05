using CustomerManagementSystem.BusinessLogic.Constants;
using System.Text.RegularExpressions;

namespace CustomerManagementSystem.BusinessLogic.Validations
{
    public class GUIDValidation
    {
        public static bool ValidateGUID(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }
            string pattern = RegexConstants.GuidRegex;
            Match regexMatch = Regex.Match(guid, pattern, RegexOptions.IgnoreCase);
            if (regexMatch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}