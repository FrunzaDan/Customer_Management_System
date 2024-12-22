using System.Text.RegularExpressions;
using CustomerManagementSystem.BusinessLogic.Constants;

namespace CustomerManagementSystem.BusinessLogic.Validations;

public class GuidValidation
{
    public static bool ValidateGuid(string guid)
    {
        if (string.IsNullOrEmpty(guid)) return false;
        var pattern = RegexConstants.GuidRegex;
        var regexMatch = Regex.Match(guid, pattern, RegexOptions.IgnoreCase);
        return regexMatch.Success;
    }
}