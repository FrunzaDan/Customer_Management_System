namespace CustomerManagementSystem.BusinessLogic.Constants
{
    public class RegexConstants
    {
        public const string GuidRegex = "^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$";
        public const string MsisdnRegex = "^[0-9]{9,12}$";
        public const string EmailRegex = "^\\S+@\\S+\\.\\S+$";
    }
}