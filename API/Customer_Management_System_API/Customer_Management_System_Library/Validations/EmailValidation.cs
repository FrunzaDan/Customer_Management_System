using Customer_Management_System_Library.Constants;
using System.Text.RegularExpressions;

namespace Customer_Management_System_Library.Validations
{
    public class EmailValidation
    {
        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            string pattern = RegexConstants.EmailRegex;
            Match regexMatch = Regex.Match(email, pattern, RegexOptions.IgnoreCase);
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