using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Linq;

namespace AzureStorageCleanup
{
    class StorageCleaner
    {
        private readonly string storageAccountName;
        private readonly string storageAccessKey;
        private readonly string containerName;
        private readonly int minDaysOld;

        public StorageCleaner(string storageAccountName, string storageAccessKey, string containerName, int minDaysOld)
        {
            this.storageAccountName = storageAccountName;
            this.storageAccessKey = storageAccessKey;
            this.containerName = containerName;
            this.minDaysOld = minDaysOld;
        }

        public void Cleanup()
        {            
            StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageAccessKey);
            CloudStorageAccount account = new CloudStorageAccount(storageCredentials, true);

            CloudBlobClient blobClient = account.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            Console.WriteLine("Deleting blob storage files in {0}\\{1} older than {2} days", storageAccountName, containerName, minDaysOld);

            DateTime referenceDate = DateTime.UtcNow;
            var blobQuery = from b in container.ListBlobs().OfType<CloudBlockBlob>()
                            where b.Properties.LastModified <= referenceDate.AddDays(-minDaysOld)
                            select b;

            var blobList = blobQuery.ToList();

            if (blobList.Count == 0)
            {
                Console.WriteLine("No files found in {0}\\{1} older than {2} days", storageAccountName, containerName, minDaysOld);
                return;
            }

            foreach (CloudBlockBlob blob in blobList)
            {
                Console.WriteLine("Deleting blob storage file {0}\\{1}, {2} days old", containerName, blob.Name, (referenceDate - blob.Properties.LastModified.Value).TotalDays);
                blob.DeleteIfExists();
            }

            Console.WriteLine("{0} blob storage files deleted in {1}\\{2} older than {3} days", blobList.Count, storageAccountName, containerName, minDaysOld);
        }
    }
}
