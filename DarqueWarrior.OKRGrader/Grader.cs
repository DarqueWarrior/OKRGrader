namespace DarqueWarrior.OKRGrader
{
   #region using
   using System.Collections.Generic;
   using System.IO;
   using System.Linq;
   using System.Net.Http;
   using System.Net.Http.Headers;
   using System.Runtime.Serialization.Json;
   using System.Text;
   using System.Threading.Tasks;
   #endregion

   /// <summary>
   /// Class the contains all the specific logic to grade an objective.
   /// Placing the code here allows it to be used anywhere in an project.
   /// </summary>
   public static class Grader
   {
      /// <summary>
      /// During development this is read from local.settings.json. Make sure
      /// update the "PAT": entry in local.settings.json to a real personal 
      /// access token to your AzD account.
      /// https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=vsts
      /// 
      /// In Azure it is read from Azure KeyVault
      /// https://medium.com/statuscode/getting-key-vault-secrets-in-azure-functions-37620fd20a0b
      /// </summary>
      private static readonly string token = System.Environment.GetEnvironmentVariable("PAT");

      /// <summary>
      /// HttpClients are expensive so only create one.
      /// </summary>
      private static readonly HttpClient client = new HttpClient();

      /// <summary>
      /// Static constructor used to setup the client to call the AzD REST API
      /// </summary>
      static Grader()
      {
         // The PAT has to be 64 bit encoded before it can be used in the
         // Authorization header.
         var encodedPat = System.Convert.ToBase64String(Encoding.UTF8.GetBytes($":{token}"));

         client.DefaultRequestHeaders.Accept.Clear();
         client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         client.DefaultRequestHeaders.Add("Authorization", $"Basic {encodedPat}");
         client.DefaultRequestHeaders.Add("User-Agent", "OKRUpdateFunction");
      }

      /// <summary>
      /// Uses the hook to request the objective work item and find all the
      /// linked key results to average. 
      /// </summary>
      /// <param name="hook"></param>
      /// <returns>AverageResult with the objective</returns>
      public static async Task<AverageResult> ProcessObjective(Hook hook)
      {
         var objectiveRef = hook.Resource.Revision.Relations.First(r => r.Rel == "System.LinkTypes.Hierarchy-Reverse");
         var requestUri = $"{objectiveRef.Url}?$Expand=relations";

         var objective = await GetWorkItemAsync<WorkItem>(requestUri);

         var average = await CalculateAverage(objective);

         // Build and return a dynamic type
         var result = new AverageResult
         {
            Objective = objective,
            Items = average.Items,
            Total = average.Total,
            Average = average.Average
         };

         return result;
      }

      /// <summary>
      /// Updates the objective with the new calculated average of all the 
      /// linked key results.
      /// </summary>
      /// <param name="objective">The objective to be updated</param>
      /// <param name="average">The new average for the objective</param>
      /// <returns>The HTTP Response from AzD</returns>
      public static async Task<HttpResponseMessage> UpdateObjective(WorkItem objective, double average)
      {
         // Build the JSON payload to send to AzD to update the objectives
         // average. The array syntax [] is not a mistake. Even though only a 
         // single property is being changed it must be sent in an array.
         var json = $"[{{\"op\":\"add\",\"path\":\"/fields/Custom.Average\",\"value\":\"{average}\"}}]";

         var content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

         // Build the URL to send the patch to.
         // We have to add bypassRules so the Average which is read only can be
         // updated anyway.
         var requestUri = $"{objective.Url}?bypassRules=true&api-version=5.0";

         var response = await client.PatchAsync(requestUri, content);

         if (!response.IsSuccessStatusCode)
         {
            throw new HttpRequestException(response.StatusCode, response.ReasonPhrase);
         }

         return response;
      }

      /// <summary>
      /// Calculates the average grade of all the linked key results.
      /// </summary>
      /// <param name="objective">The objective to score</param>
      /// <returns>AverageResult without the objective</returns>
      private static async Task<AverageResult> CalculateAverage(WorkItem objective)
      {
         // Request all the work items at the same time.
         IEnumerable<Task<KeyResult>> asyncOps = objective.Relations
            .Where(r => r.Rel == "System.LinkTypes.Hierarchy-Forward")
            .Select(r => GetWorkItemAsync<KeyResult>($"{r.Url}?$Expand=relations"));

         var workitems = await Task.WhenAll(asyncOps);

         int items = 0;
         double total = 0;

         foreach (var workitem in workitems)
         {
            // They may link items to the objective like tasks or user stories.
            // This makes sure we only count the key results.
            if (workitem.WorkItemType == KeyResult.WorkItemTypeValue)
            {
               items++;
               total += workitem.Grade;
            }
         }

         // Build and return a dynamic type
         var result = new AverageResult
         {
            Items = items,
            Total = total,
            Average = total / items
         };

         return result;
      }

      /// <summary>
      /// Used to request objective and key results from AzD
      /// </summary>
      /// <typeparam name="T">Type of item to deserialize and return</typeparam>
      /// <param name="requestUri">The URL to the item to request</param>
      /// <returns>The deserialized T</returns>
      public static async Task<T> GetWorkItemAsync<T>(string requestUri) where T : class
      {
         var serializer = new DataContractJsonSerializer(typeof(T));

         var stream = await GetWorkItemSteam(requestUri);

         return serializer.ReadObject(stream) as T;
      }

      /// <summary>
      /// Returns a steam of the request from AzD to pass to the serializer.
      /// </summary>
      /// <param name="requestUri">The URL to the item to request</param>
      /// <returns>A stream that can be passed to the serializer.</returns>
      public static async Task<Stream> GetWorkItemSteam(string requestUri)
      {
         client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         var response = await client.GetAsync(requestUri);

         if (!response.IsSuccessStatusCode)
         {
            throw new HttpRequestException(response.StatusCode, response.ReasonPhrase);
         }

         var stream = await response.Content.ReadAsStreamAsync();

         // AzD will return HTML with an error message in it for some error 
         // conditions. When that happens the response code is a 203
         // (NonAuthoritativeInformation). This will result in a parsing error
         // because the HTML can't be parsed as JSON. Log this now so it will
         // aid in debugging error
         if (response.StatusCode == System.Net.HttpStatusCode.NonAuthoritativeInformation)
         {
            using (TextReader steamReader = new StreamReader(stream))
            {
               var text = await steamReader.ReadToEndAsync();

               if(text.Contains("Sign In"))
               {
                  // The response is a sign in page. This happens when the PAT can't be parsed.
                  throw new HttpRequestException(System.Net.HttpStatusCode.Unauthorized, "Check your Personal Access Token");
               }
               else
               {
                  // Rewind the stream so it can be read downstream.
                  stream.Position = 0;
               }
            }
         }

         return stream;
      }
   }
}