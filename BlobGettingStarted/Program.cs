
namespace BlobGettingStarted
{
    using System;
    using System.IO;
    using System.Configuration;
    using Microsoft.Azure.KeyVault;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.Azure;
    public class Program
    {
        const string DemoContainer = "democontainer";
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = EncryptionShared.Utility.CreateStorageAccountFromConnectionString();
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(CloudConfigurationManager.GetSetting("Container"));
            try
            {
                container.CreateIfNotExists();
                
                Console.WriteLine("Blob encryption sample");
                int size = 5 * 1024 * 1024;
                byte[] buffer = new byte[size];
                Random rand = new Random();
                rand.NextBytes(buffer);
                CloudBlockBlob blob = container.GetBlockBlobReference("blockblob");
                blob.CreateSnapshot();
                // Create the IKey used for encryption.
                RsaKey key = new RsaKey("private:key1");

                // Create the encryption policy to be used for upload.
                BlobEncryptionPolicy uploadPolicy = new BlobEncryptionPolicy(key, null);

                // Set the encryption policy on the request options.
                BlobRequestOptions uploadOptions = new BlobRequestOptions() { EncryptionPolicy = uploadPolicy };

                Console.WriteLine("Uploading the encrypted blob.");

                // Upload the encrypted contents to the blob.
                using (MemoryStream stream = new MemoryStream(buffer))
                {
                    blob.UploadFromStream(stream, size, null, uploadOptions, null);
                }

                // Download the encrypted blob.
                // For downloads, a resolver can be set up that will help pick the key based on the key id.
                LocalResolver resolver = new LocalResolver();
                resolver.Add(key);

                BlobEncryptionPolicy downloadPolicy = new BlobEncryptionPolicy(null, resolver);

                // Set the decryption policy on the request options.
                BlobRequestOptions downloadOptions = new BlobRequestOptions() { EncryptionPolicy = downloadPolicy };

                Console.WriteLine("Downloading the encrypted blob.");

                // Download and decrypt the encrypted contents from the blob.
                using (MemoryStream outputStream = new MemoryStream())
                {
                    blob.DownloadToStream(outputStream, null, downloadOptions, null);
                }
                Console.WriteLine("Press enter key to exit");
                //Console.ReadLine();
                #region folder
                //folder creation
                CloudBlobDirectory directory = container.GetDirectoryReference("directoryName");
                CloudBlockBlob blockblob = directory.GetBlockBlobReference("sample");
                blockblob.UploadFromFile(@"C:\Users\RINGEL\Downloads\Angular.txt", FileMode.Open);
                //listof Directories &files
                DirectoryFile(container);
                #endregion folder

                #region  sas
                var sasPolicy = new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessStartTime = DateTime.Now.AddMinutes(-10),
                    SharedAccessExpiryTime = DateTime.Now.AddMinutes(30)
                };
                var sasTokenforcontainer = container.GetSharedAccessSignature(sasPolicy);
                string sasTokenforblob = blob.GetSharedAccessSignature(sasPolicy);
                #endregion sas
            }
            finally
            {
                container.DeleteIfExists();
            }
        }

        private static void DirectoryFile(CloudBlobContainer container)
        {
            foreach (var item in container.ListBlobs(null, false))
            {
                dynamic name = item;
                if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory subFolder = (CloudBlobDirectory)item;
                    Console.WriteLine("Is a folder with name " + subFolder.Prefix);
                    foreach (var sa in subFolder.ListBlobs())
                    {
                        if (sa.GetType() == typeof(CloudBlobDirectory))
                        {
                            CloudBlobDirectory subFolder1 = (CloudBlobDirectory)item;
                             Console.WriteLine("Is a folder with name " + subFolder1.Prefix);
                        }
                        else if (item.GetType() == typeof(CloudBlockBlob))
                        {
                            Console.WriteLine("Is a file with name" + name.Name);
                        }
                    }
                }
                else
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        Console.WriteLine("Is a file with name" + name.Name);
                    }
                }
            }
        }
    }
}