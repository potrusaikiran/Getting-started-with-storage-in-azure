namespace EncryptionShared
{
    using System;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage;
    public static class Utility
    {
        public static CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            string storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }
    }
}
