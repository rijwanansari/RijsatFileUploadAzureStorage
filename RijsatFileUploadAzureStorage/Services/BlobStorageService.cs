using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;

namespace RijsatFileUploadAzureStorage.Services;
public class BlobStorageService : IBlobStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BlobStorageService> _logger;
    string blobStorageconnection = string.Empty;
    private string blobContainerName = "rijsatdemo";
    public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        blobStorageconnection = _configuration.GetConnectionString("AzureStorageAccount");
    }
    public async Task<string> UploadFileToBlobAsync(string strFileName, string contecntType, Stream fileStream)
    {
        try
        {
            var container = new BlobContainerClient(blobStorageconnection, blobContainerName);
            var createResponse = await container.CreateIfNotExistsAsync();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            var blob = container.GetBlobClient(strFileName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contecntType });
            var urlString = blob.Uri.ToString();
            return urlString;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.ToString());
            throw;
        }
    }
    public async Task<bool> DeleteFileToBlobAsync(string strFileName)
    {
        try
        {
            var container = new BlobContainerClient(blobStorageconnection, blobContainerName);
            var createResponse = await container.CreateIfNotExistsAsync();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            var blob = container.GetBlobClient(strFileName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.ToString());
            throw;
        }
    }

    public async Task<string> GetBlobSASTOkenByFile(string fileName)
    {
        try
        {
            var azureStorageAccount = _configuration.GetSection("AzureStorage:AzureAccount").Value;
            var azureStorageAccessKey = _configuration.GetSection("AzureStorage:AccessKey").Value;
            Azure.Storage.Sas.BlobSasBuilder blobSasBuilder = new Azure.Storage.Sas.BlobSasBuilder()
            {
                BlobContainerName = blobContainerName,
                BlobName = fileName,
                ExpiresOn = DateTime.UtcNow.AddMinutes(2),//Let SAS token expire after 5 minutes.
            };
            blobSasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Read);//User will only be able to read the blob and it's properties
            var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(azureStorageAccount,
                azureStorageAccessKey)).ToString();
            return sasToken;
        }
        catch (Exception)
        {

            throw;
        }
    }

}

