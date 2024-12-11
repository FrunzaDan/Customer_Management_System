using System.Text.RegularExpressions;
using CustomerManagementSystem.BusinessLogic.Constants;

namespace CustomerManagementSystem.BusinessLogic.Validations;

public class GUIDValidation
{
    public static bool ValidateGUID(string guid)
    {
        if (string.IsNullOrEmpty(guid)) return false;
        var pattern = RegexConstants.GuidRegex;
        var regexMatch = Regex.Match(guid, pattern, RegexOptions.IgnoreCase);
        if (regexMatch.Success)
            return true;
        return false;
    }
}