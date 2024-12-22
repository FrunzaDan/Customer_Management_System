using System.Text.RegularExpressions;
using CustomerManagementSystem.BusinessLogic.Constants;

namespace CustomerManagementSystem.BusinessLogic.Validations;

public static class EmailValidation
{
    public static bool ValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return false;

        var pattern = RegexConstants.EmailRegex;
        var regexMatch = Regex.Match(email, pattern, RegexOptions.IgnoreCase);
        return regexMatch.Success;
    }
}