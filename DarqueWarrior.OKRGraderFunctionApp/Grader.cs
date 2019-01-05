namespace DarqueWarrior.OKRGraderFunctionApp
{
   #region using
   using DarqueWarrior.OKRGrader;
   using Microsoft.AspNetCore.Http;
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.Azure.WebJobs;
   using Microsoft.Azure.WebJobs.Extensions.Http;
   using Microsoft.Extensions.Logging;
   using System.Runtime.Serialization.Json;
   using System.Threading.Tasks;
   #endregion

   public static class Grader
   {
      /// <summary>
      /// The purpose of this function is to parse the request body and pass
      /// to the Grader. The Grader will communicate with Azure DevOps (AzD)
      /// and calculate the average for the objective based on the link key 
      /// results.
      /// </summary>
      /// <param name="req">Request from the AzD web hook</param>
      /// <param name="log">Logger</param>
      /// <returns></returns>
      [FunctionName("Grader")]
      public static async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
          ILogger log)
      {
         log.LogInformation("C# HTTP trigger function processed an AzD web hook request.");

         // Setup a JSON serializer to parse the body into our Hook POCO.
         var serializer = new DataContractJsonSerializer(typeof(Hook));

         if (!(serializer.ReadObject(req.Body) is Hook hook))
         {
            return new BadRequestObjectResult("Error parsing the Hook JSON");
         }

         // Holds the results of processing the objective. This is used below
         // to update the objective.
         AverageResult result;

         try
         {
            log.LogInformation($"Attempt to process hook");
            
            // Throws if at any point it does not get a 200 status code from AzD
            result = await OKRGrader.Grader.ProcessObjective(hook);

            log.LogInformation($"Found {result.Items} Key Results with a total of {result.Total} and an average of {result.Average}");

            log.LogInformation($"Attempt to update objective");

            // Throws if at any point it does not get a 200 status code from AzD
            var response = await OKRGrader.Grader.UpdateObjective(result.Objective, result.Average);

            log.LogInformation($"Update Objective Response {response.Content}");

            // By this point it is safe to return OK because if any status code
            // other than OK was returned an exception was thrown and handled
            // below.
            return new OkResult();
         }
         catch (HttpRequestException rex)
         {
            log.LogError("Error accessing AzD.");
            log.LogError(rex.Message);

            return new StatusCodeResult((int)rex.StatusCode);
         }
         catch (System.Exception ex)
         {
            log.LogError("Error processing the hook.");
            log.LogError(ex.Message);

            return new BadRequestResult();
         }
      }
   }
}