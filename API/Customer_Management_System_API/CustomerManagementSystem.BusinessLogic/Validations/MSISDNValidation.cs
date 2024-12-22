using System.Text.RegularExpressions;
using CustomerManagementSystem.BusinessLogic.Constants;

namespace CustomerManagementSystem.BusinessLogic.Validations;

public static class MsisdnValidation
{
    public static bool ValidateMsisdn(string msisdn)
    {
        if (string.IsNullOrEmpty(msisdn)) return false;
        const string pattern = RegexConstants.MsisdnRegex;
        var regexMatch = Regex.Match(msisdn, pattern, RegexOptions.IgnoreCase);
        return regexMatch.Success;
    }
}