namespace Sharecode.Backend.Application.Client;

public interface IFileClient
{
    Task<(bool, Uri?)> UploadFileAsync(string fileName, byte[] fileObject, bool overwrite = false, CancellationToken token = default);

    Task<byte[]?> GetFileAsync(string fileName);

    Task<bool> DeleteFileAsync(string fileName);
}