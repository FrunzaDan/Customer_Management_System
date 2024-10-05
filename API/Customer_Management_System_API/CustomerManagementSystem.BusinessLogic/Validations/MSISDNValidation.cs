﻿using CustomerManagementSystem.BusinessLogic.Constants;
using System.Text.RegularExpressions;

namespace CustomerManagementSystem.BusinessLogic.Validations
{
    public class MSISDNValidation
    {
        public static bool ValidateMsisdn(string msisdn)
        {
            if (string.IsNullOrEmpty(msisdn))
            {
                return false;
            }
            string pattern = RegexConstants.MsisdnRegex;
            Match regexMatch = Regex.Match(msisdn, pattern, RegexOptions.IgnoreCase);
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