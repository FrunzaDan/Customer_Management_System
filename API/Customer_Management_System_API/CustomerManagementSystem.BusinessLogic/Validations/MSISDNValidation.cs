using System.Text.RegularExpressions;
using CustomerManagementSystem.BusinessLogic.Constants;

namespace CustomerManagementSystem.BusinessLogic.Validations;

public class MSISDNValidation
{
    public static bool ValidateMsisdn(string msisdn)
    {
        if (string.IsNullOrEmpty(msisdn)) return false;
        var pattern = RegexConstants.MsisdnRegex;
        var regexMatch = Regex.Match(msisdn, pattern, RegexOptions.IgnoreCase);
        if (regexMatch.Success)
            return true;
        return false;
    }
}