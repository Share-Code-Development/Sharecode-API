namespace Sharecode.Backend.Utilities.Email;

public static class EmailPlaceholderKeys
{

    #region Common

    public static string UserNameKey => "USER";
    public static string UserEmailKey => "EMAIL";
    public static string GatewayUrlKey => "GATEWAY_URL";

    #endregion

    #region AccountLocked

    public static string AccountLockedLastAttemptKey => "LAST_ATTEMPT_OCCURENCE";
    public static string AccountLockedIpAddress => "IP_ADDRESS";
    public static string AccountLockedCountry => "COUNTRY";

    #endregion

}