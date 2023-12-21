using Microsoft.Extensions.Options;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Utilities.Configuration;

namespace Sharecode.Backend.Infrastructure.Client;

public class LocalFileClient(IOptions<FileClientConfiguration> fileClientConfiguration, ILogger? logger) : IFileClient
{
    private readonly string _parentPath = fileClientConfiguration.Value.ClientType == "Local" ? fileClientConfiguration.Value.Local?.FilePath ?? throw new ApplicationException("No client configuration provided for FilePath") : throw new ApplicationException($"Client is not configured for Local Files");
    private readonly string _parentUrl = fileClientConfiguration.Value.ClientType == "Local" ? fileClientConfiguration.Value.Local?.FileUrl ?? throw new ApplicationException("No client configuration provided for FileUrl") : throw new ApplicationException($"Client is not configured for Local Files");
    private readonly ILogger _logger = logger ?? Log.ForContext<LocalFileClient>();

    public async Task<(bool, Uri?)> UploadFileAsync(string fileName, byte[] fileObject, bool overwrite = false, CancellationToken token = default)
    {
        try
        {
            var consolidatedFileName = GetFullPath(fileName);
            if (File.Exists(consolidatedFileName) && !overwrite)
            {
                return (false, new Uri($"{_parentUrl}/{fileName}"));
            }

            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            File.WriteAllBytes(consolidatedFileName, fileObject);
            return (true, new Uri($"{_parentUrl}/{fileName}"));
        }
        catch (Exception e)
        {
            _logger.Error(e , "Failed to upload file {FileName} on {ParentPath} due to {Message}", fileName, _parentPath, e.Message);
            return (false, null);
        }

        return (false, null);
    }

    public async Task<byte[]?> GetFileAsync(string fileName)
    {
        var consolidatedFileName = GetFullPath(fileName);
        if (!File.Exists(consolidatedFileName))
        {
            return null;
        }

        return await File.ReadAllBytesAsync(consolidatedFileName);
    }

    public async Task<bool> DeleteFileAsync(string fileName)
    {
        try
        {
            var consolidatedFileName = GetFullPath(fileName);
            File.Delete(fileName);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to delete file {FileName}", fileName);
            return false;
        }
    }

    private string GetFullPath(string fileName)
    {
        return Path.Combine(_parentPath, fileName);
    }
}