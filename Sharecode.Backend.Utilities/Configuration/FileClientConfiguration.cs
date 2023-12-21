namespace Sharecode.Backend.Utilities.Configuration;

public class FileClientConfiguration
{
    public string ClientType { get; set; }
    public LocalClientConfiguration? Local { get; set; }
    public OracleOciConfiguration? Oci { get; set; }
}

public class LocalClientConfiguration
{
    public string FilePath { get; set; }
    public string FileUrl { get; set; }
}

public class OracleOciConfiguration
{
    
}