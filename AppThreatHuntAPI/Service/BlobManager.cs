using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppThreatHuntAPI
{
    public class BlobManager
    {
        private BlobContainerClient jsonBlobContainerClient = null;

        private void InitializeJSONContainerClient()
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("BlobConnString__blobServiceUri");
            string jsonContainer = "hunt";
            jsonBlobContainerClient = new BlobContainerClient(storageConnectionString, jsonContainer);
        }
        public void SaveJSONToBlob(string jsonString, string jsonFileName)
        {
            if (jsonBlobContainerClient == null)
            {
                InitializeJSONContainerClient();
            }
            try
            {
                BlobClient blobClient = jsonBlobContainerClient.GetBlobClient(jsonFileName);
                using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString)))
                {
                    blobClient.Upload(stream, true);
                }
            }
            catch (Exception ex)
            {
            }

        }
        public string GetJsonFromBlob(string jsonFileName)
        {
            string jsonString = "";

            if (jsonBlobContainerClient == null)
            {
                InitializeJSONContainerClient();
            }
            try
            {
                BlobClient blobClient = jsonBlobContainerClient.GetBlobClient(jsonFileName);
                BlobDownloadInfo blobDownloadInfo = blobClient.Download();
                using (MemoryStream stream = new MemoryStream())
                {
                    blobDownloadInfo.Content.CopyTo(stream);
                    // Reset the stream position to the beginning
                    stream.Position = 0;
                    // Deserialize the JSON from the stream
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonString = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return jsonString;

        }
        public void DeleteJsonFile(string jsonFileName)
        {

            if (jsonBlobContainerClient == null)
            {
                InitializeJSONContainerClient();
            }
            try
            {
                var blobClient = jsonBlobContainerClient.GetBlobClient(jsonFileName);
                if (blobClient.Exists())
                {
                    blobClient.Delete();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
