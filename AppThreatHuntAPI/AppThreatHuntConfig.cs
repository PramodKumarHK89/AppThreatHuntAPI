using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Core;

namespace AppThreatHuntAPI
{
    public static class AppThreatHuntConfig
    {
        [FunctionName("AppThreatHuntConfig")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (req.Method=="GET")
            {
                BlobManager blobManager = new BlobManager();
                Policy policy = JsonConvert.DeserializeObject<Policy>(blobManager.GetJsonFromBlob("Policy.json"));
                return new OkObjectResult(policy);
            }
            else
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Policy policy = JsonConvert.DeserializeObject<Policy>(requestBody);
                BlobManager blobManager = new BlobManager();
                blobManager.SaveJSONToBlob(JsonConvert.SerializeObject(policy), "Policy.json");
                return new OkObjectResult(policy);
            }

        }
    }
}
