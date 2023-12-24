namespace Sharecode.Backend.Application.Client;

public interface IFileClient
{
    Task<(bool, Uri?)> UploadFileAsync(string fileName, byte[] fileObject, bool overwrite = false, CancellationToken token = default);

    Task<byte[]?> GetFileAsync(string fileName, CancellationToken token = default);

    Task<bool> DeleteFileAsync(string fileName, CancellationToken token = default);
    
    Task<string?> GetFileAsStringAsync(string fileName, CancellationToken token = default);
    Task<byte[]> GetChecksum(string fileName);
    Task<byte[]> GetChecksum(byte[] file);
    bool CompareChecksums(byte[] storedChecksum, byte[] calculatedChecksum)
    {
        return storedChecksum.SequenceEqual(calculatedChecksum);
    }
}