﻿namespace TableGettingStartedUsingAttributes
{
    using System;
    using System.Reflection;
    using Microsoft.Azure.KeyVault;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json.Linq;
    /// <summary>
    /// Demonstrates how to use encryption with the Azure Table service.
    /// </summary>
    public class Program
    {
        const string DemoTable = "demotable";

        static void Main(string[] args)
        {
            Console.WriteLine("Table encryption sample");

            // Retrieve storage account information from connection string
            // How to create a storage connection string - https://azure.microsoft.com/en-us/documentation/articles/storage-configure-connection-string/
            CloudStorageAccount storageAccount = EncryptionShared.Utility.CreateStorageAccountFromConnectionString();
            CloudTableClient client = storageAccount.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(DemoTable + Guid.NewGuid().ToString("N"));

            try
            {
                table.Create();

                // Create the IKey used for encryption.
                RsaKey key = new RsaKey("private:key1");

                EncryptedEntity ent = new EncryptedEntity() { PartitionKey = Guid.NewGuid().ToString(), RowKey = DateTime.Now.Ticks.ToString() };
                ent.Populate();

                TableRequestOptions insertOptions = new TableRequestOptions()
                {
                    EncryptionPolicy = new TableEncryptionPolicy(key, null)
                };

                // Insert Entity
                Console.WriteLine("Inserting the encrypted entity.");
                table.Execute(TableOperation.Insert(ent), insertOptions, null);
                // For retrieves, a resolver can be set up that will help pick the key based on the key id.
                Console.WriteLine("inserting keys are partiation" + ent.PartitionKey + "rowkey" + ent.RowKey);
                LocalResolver resolver = new LocalResolver();
                resolver.Add(key);

                TableRequestOptions retrieveOptions = new TableRequestOptions()
                {
                    EncryptionPolicy = new TableEncryptionPolicy(null, resolver)
                };

                // Retrieve Entity
                Console.WriteLine("Retrieving the encrypted entity.");
                TableOperation operation = TableOperation.Retrieve(ent.PartitionKey, ent.RowKey);
                TableResult result = table.Execute(operation, retrieveOptions, null);
                var sa = result.Result;
                //JObject json = JObject.FromObject(sa);
                //foreach (JProperty property in json.Properties())
                //    Console.WriteLine(property.Name + " - " + property.Value);
                var properties = GetProperties(sa);
                foreach (var p in properties)
                {
                    string name = p.Name;
                    var value = p.GetValue(sa, null);
                    Console.WriteLine("name is " + name);
                    Console.WriteLine("value is " + value);
                }
                 Console.WriteLine("Press enter key to exit");
                Console.ReadLine();
            }
            finally
            {
                table.DeleteIfExists();
            }
        }
        private static PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties();
        }

    }
}