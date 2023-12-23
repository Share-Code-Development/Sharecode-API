using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Extensions.Options;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Utilities.Configuration;

namespace Sharecode.Backend.Infrastructure.Client;

public class LocalFileClient(IOptions<FileClientConfiguration> fileClientConfiguration, ILogger? logger) : IFileClient
{
    private readonly string _parentPath = fileClientConfiguration.Value.ClientType == "Local" ? fileClientConfiguration.Value.Local?.FilePath ?? throw new ApplicationException("No client configuration provided for FilePath") : throw new ApplicationException($"Client is not configured for Local Files");
    private readonly string _parentUrl = fileClientConfiguration.Value.ClientType == "Local" ? fileClientConfiguration.Value.Local?.FileUrl ?? throw new ApplicationException("No client configuration provided for FileUrl") : throw new ApplicationException($"Client is not configured for Local Files");
    private readonly bool _revokeExecutePermission = fileClientConfiguration.Value.ClientType == "Local" ? fileClientConfiguration.Value.Local?.RevokeExecutePermission ?? throw new ApplicationException("No client configuration provided for FileUrl") : throw new ApplicationException($"Client is not configured for Local Files");
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
            if (_revokeExecutePermission)
            {
                bool success = false;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    success = await RevokeExecutePermissionWindows(consolidatedFileName);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    success = await RevokeExecutePermissionLinux(consolidatedFileName);
                }

                if (success)
                {
                    _logger.Information("Removed the execute permission of the file @ {Path}", consolidatedFileName);
                }
                else
                {
                    _logger.Warning("Failed to revoke the execute permission of the file @ {Path}", consolidatedFileName);
                }
            }
            return (true, new Uri($"{_parentUrl}/{fileName}"));
        }
        catch (Exception e)
        {
            _logger.Error(e , "Failed to upload file {FileName} on {ParentPath} due to {Message}", fileName, _parentPath, e.Message);
            return (false, null);
        }

        return (false, null);
    }

    public async Task<byte[]?> GetFileAsync(string fileName, CancellationToken token = default)
    {
        var consolidatedFileName = GetFullPath(fileName);
        if (!File.Exists(consolidatedFileName))
        {
            return null;
        }

        return await File.ReadAllBytesAsync(consolidatedFileName, token);
    }

    public async Task<bool> DeleteFileAsync(string fileName, CancellationToken token = default)
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

    public async Task<string?> GetFileAsStringAsync(string fileName, CancellationToken token = default)
    {
        var consolidatedFileName = GetFullPath(fileName);
        if (!File.Exists(consolidatedFileName))
        {
            return null;
        }

        return await File.ReadAllTextAsync(consolidatedFileName, token);
    }

    private string GetFullPath(string fileName)
    {
        return Path.Combine(_parentPath, fileName);
    }
    
    private async Task<bool> RevokeExecutePermissionWindows(string consolidatedFileName)
    {
        try
        {
            var fileInfo = new FileInfo(consolidatedFileName);
            var accessControl = fileInfo.GetAccessControl();
            accessControl.RemoveAccessRule(new FileSystemAccessRule(
                $"{Environment.UserDomainName}\\{Environment.UserName}", FileSystemRights.ExecuteFile, AccessControlType.Allow));
            fileInfo.SetAccessControl(accessControl);
            return true;
        }
        catch(Exception e)
        {
            _logger.Error(e, "Failed to revoke the permission on {OperatingSystem} of file {FileName} due to {Message}", "Windows", consolidatedFileName, e.Message);
            return false;
        }
    }

    private async Task<bool> RevokeExecutePermissionLinux(string consolidatedFileName)
    {
        try
        {
            var procStartInfo = new ProcessStartInfo("bash", $"chmod a-x {consolidatedFileName}")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var proc = new Process
            {
                StartInfo = procStartInfo
            };

            proc.Start();
            // Wait for the process to end
            await proc.WaitForExitAsync();

            // Returns true if the process ended correctly (exit code 0)
            return proc.ExitCode == 0;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to revoke the permission on {OperatingSystem} of file {FileName} due to {Message}", "Linux", consolidatedFileName, e.Message);
            throw;
        }
    }
}