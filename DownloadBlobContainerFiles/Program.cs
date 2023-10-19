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

        bool? yesToAll = null;

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
            BlobProperties properties = await blobClient.GetPropertiesAsync();

            Console.WriteLine($"File: {blobItem.Name}, Size: {properties.ContentLength} bytes");

            if (yesToAll == null)
            {
                Console.WriteLine("Download this file? (y/n/yall/nall)");
                var choice = Console.ReadLine().ToLower();

                switch (choice)
                {
                    case "y":
                        await DownloadBlob(blobClient);
                        break;
                    case "n":
                        break;
                    case "yall":
                        yesToAll = true;
                        await DownloadBlob(blobClient);
                        break;
                    case "nall":
                        yesToAll = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Skipping...");
                        break;
                }
            }
            else if (yesToAll == true)
            {
                await DownloadBlob(blobClient);
            }
        }

        Console.WriteLine("Operation complete!");
    }

    private static async Task DownloadBlob(BlobClient blobClient)
    {
        Console.WriteLine($"Downloading {blobClient.Name}...");

        BlobDownloadInfo download = await blobClient.DownloadAsync();

        using (FileStream fs = File.OpenWrite(Path.Combine(destinationPath, blobClient.Name)))
        {
            await download.Content.CopyToAsync(fs);
            fs.Close();
        }
    }
}
