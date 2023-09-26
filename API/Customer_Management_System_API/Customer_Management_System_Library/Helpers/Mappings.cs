namespace Customer_Management_System_Library.Helpers
{
    public class Mappings
    {
        public static readonly Dictionary<int, string> SQLResponseDictionary = new Dictionary<int, string>
        {
            { 200, "Success!" },
            { 4001, "MSISDN already exists!" },
            { 4002, "Email already exists!" }
        };

        public static readonly Dictionary<int, string> ErrorCodes = new Dictionary<int, string>
        {
            { 200, "Success!" },
            { 4001, "MSISDN already exists!" },
            { 4002, "Email already exists!" }
        };

        public static readonly Dictionary<int, string> Roles = new Dictionary<int, string>
        {
            { 1801, "Administrator" },
            { 1802, "Customer" },
            { 1803, "NoRights"}
        };

        public static readonly Dictionary<int, string> CustomerStatus = new Dictionary<int, string>
        {
            { 1901, "Active" },
            { 1902, "Inactive" },
            { 1903, "Deactivated" },
            { 1904 , "Banned"}
        };

        public static readonly Dictionary<int, string> SearchOption = new Dictionary<int, string>
        {
            { 1, "GUID" },
            { 2, "MSISDN" },
            { 3, "Email" }
        };
    }
}

