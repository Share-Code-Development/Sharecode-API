namespace Sharecode.Backend.Utilities;

public class KeyVaultConstants
{
    #region Jwt

    public static string JwtSecretKey => "jwt-secret-key";
    public static string JwtRefreshTokenSecretKey => "jwt-secret-key-refresh";

    #endregion

    #region Redis

    public static string RedisConnectionDbName => "redis-connection-db-name";
    public static string RedisConnectionPassword => "redis-connection-password";
    public static string RedisConnectionString => "redis-connection-string";
    public static string RedisConnectionUserName => "redis-connection-user-name";

    #endregion

    #region SQL Server

    public static string SqlServerDataSource => "sql-server-data-source";
    public static string SqlServerInitialCatalog => "sql-server-initial-catalog";
    public static string SqlServerPassword => "sql-server-password";
    public static string SqlServerUserId => "sql-server-user-id";

    #endregion

    #region Smtp
    public static string SmtpHost => "smtp-host";
    public static string SmtpUserName => "smtp-user";
    public static string SmtpPassword => "smtp-password";
    public static string SmtpPort => "smtp-port";
    public static string SmtpFrom => "smtp-from";
    public static string SmtpFromName => "smtp-from-name";

    #endregion

    #region PSQL

    public static string PsDatabase => "ps-database";
    public static string PsHost => "ps-host";
    public static string PsPort => "ps-port";
    public static string PsUserName => "ps-username";
    public static string PsPassword => "ps-password";

    #endregion

    #region Google Secrets
    public static string GoogleClientId => "google-client-id";
    public static string GoogleClientSecret => "google-client-secret";

    #endregion
}