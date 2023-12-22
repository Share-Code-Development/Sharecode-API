namespace Sharecode.Backend.Application.Client;

public interface IFileClient
{
    Task<(bool, Uri?)> UploadFileAsync(string fileName, byte[] fileObject, bool overwrite = false, CancellationToken token = default);

    Task<byte[]?> GetFileAsync(string fileName, CancellationToken token = default);

    Task<bool> DeleteFileAsync(string fileName, CancellationToken token = default);
    
    Task<string?> GetFileAsStringAsync(string fileName, CancellationToken token = default);
}