namespace CustomerManagementSystem.BusinessLogic.Helpers;

public class Mappings
{
    public static readonly Dictionary<int, string> SqlResponseDictionary = new()
    {
        { 200, "Success!" },
        { 4001, "MSISDN already exists!" },
        { 4002, "Email already exists!" }
    };

    public static readonly Dictionary<int, string> ErrorCodes = new()
    {
        { 200, "Success!" },
        { 4001, "MSISDN already exists!" },
        { 4002, "Email already exists!" }
    };

    public static readonly Dictionary<int, string> Roles = new()
    {
        { 1801, "Administrator" },
        { 1802, "Customer" },
        { 1803, "NoRights" }
    };

    public static readonly Dictionary<int, string> CustomerStatus = new()
    {
        { 1901, "Active" },
        { 1902, "Inactive" },
        { 1903, "Deactivated" },
        { 1904, "Banned" }
    };

    public static readonly Dictionary<int, string> SearchOption = new()
    {
        { 1, "GUID" },
        { 2, "MSISDN" },
        { 3, "Email" }
    };
}