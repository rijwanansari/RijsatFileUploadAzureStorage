namespace RijsatFileUploadAzureStorage.Services;
public interface IBlobStorageService
{
    Task<string> UploadFileToBlobAsync(string strFileName, string contecntType, Stream fileStream);
    Task<bool> DeleteFileToBlobAsync(string strFileName);
    Task<string> GetBlobSASTOkenByFile(string fileName);
}