using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AzureStorageCleanup
{
    class StorageCleaner
    {
        private readonly string storageAccountName;
        private readonly string storageAccessKey;
        private readonly string containerName;
        private readonly int minDaysOld;
        private readonly string searchPattern;
        private readonly bool recursive;
        private readonly bool whatIf;

        public StorageCleaner(
            string storageAccountName,
            string storageAccessKey,
            string containerName,
            int minDaysOld,
            string searchPattern,
            bool recursive,
            bool whatIf)
        {
            this.storageAccountName = storageAccountName;
            this.storageAccessKey = storageAccessKey;
            this.containerName = containerName;
            this.minDaysOld = minDaysOld;
            this.searchPattern = searchPattern;
            this.recursive = recursive;
            this.whatIf = whatIf;
        }

        public void Cleanup()
        {
            string whatifTxt = this.whatIf ? "[WHATIF] " : "";

            StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageAccessKey);
            CloudStorageAccount account = new CloudStorageAccount(storageCredentials, true);

            CloudBlobClient blobClient = account.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            Console.WriteLine("{0}Deleting blob storage files in {1}/{2} older than {3} days", whatifTxt, storageAccountName, containerName, minDaysOld);

            DateTime referenceDate = DateTime.UtcNow;
            var blobQuery = from b in container.ListBlobs(null, recursive).OfType<ICloudBlob>()
                            where b.Properties.LastModified <= referenceDate.AddDays(-minDaysOld)
                                && Regex.IsMatch(b.Name, searchPattern)
                            select b;

            var blobList = blobQuery.ToList();

            if (blobList.Count == 0)
            {
                Console.WriteLine("{0}No files found in {1}/{2} older than {3} days", whatifTxt, storageAccountName, containerName, minDaysOld);
                return;
            }

            foreach (ICloudBlob blob in blobList)
            {
                double blobAgeInDays = (referenceDate - blob.Properties.LastModified.Value).TotalDays;
                Console.WriteLine("{0}Deleting blob storage file {1}/{2}, {3} days old", whatifTxt, containerName, blob.Name, Math.Round(blobAgeInDays, 3));

                if (!whatIf)
                {
                    blob.DeleteIfExists();
                }
            }

            Console.WriteLine("{0}{1} blob storage files deleted in {2}/{3} older than {4} days", whatifTxt, blobList.Count, storageAccountName, containerName, minDaysOld);
        }
    }
}
