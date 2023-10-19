using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

class Program
{
    private const string connectionString = "YOUR_AZURE_STORAGE_CONNECTION_STRING";
    private const string containerName = "YOUR_CONTAINER_NAME";
    private const string destinationPath = "YOUR_LOCAL_DESTINATION_PATH";

    static async Task Main(string[] args)
    {
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);

            Console.WriteLine($"Downloading {blobItem.Name}...");

            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using (FileStream fs = File.OpenWrite(Path.Combine(destinationPath, blobItem.Name)))
            {
                await download.Content.CopyToAsync(fs);
                fs.Close();
            }
        }

        Console.WriteLine("Download complete!");
    }
}