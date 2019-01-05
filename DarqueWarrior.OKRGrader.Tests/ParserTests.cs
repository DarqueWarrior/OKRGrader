namespace DarqueWarrior.OKRGrader.Tests
{
   #region using
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using System.IO;
   using System.Runtime.Serialization.Json;
   using DarqueWarrior.OKRGrader;
   #endregion

   [TestClass]
   public class ParserTests
   {
      /// <summary>
      ///  Test that the classes properly parse a key result from the AzD 
      ///  REST API
      /// </summary>
      [TestMethod]
      [DeploymentItem("SampleFiles/keyresult.json")]
      public void Parse_KeyResult()
      {
         // Arrange
         MemoryStream stream = LoadJSON("SampleFiles/keyresult.json");

         // Act
         var serializer = new DataContractJsonSerializer(typeof(KeyResult));
         var workItem = serializer.ReadObject(stream) as KeyResult;

         // Assert
         Assert.AreEqual(91, workItem.Id, 
            "The work item id does not match");

         Assert.AreEqual(0.5, workItem.Grade,
            "The grade on the work item does not match");

         Assert.AreEqual("https://dev.azure.com/unittests/redacted/_apis/wit/workItems/91", workItem.Url.ToString(),
            "The URL on the work item does not match");

         Assert.AreEqual("System.LinkTypes.Hierarchy-Reverse", workItem.Relations[0].Rel,
            "The first related work item type does not match");

         Assert.AreEqual("Key Result", workItem.WorkItemType,
            "The work item type does not match.");
      }

      /// <summary>
      ///  Test that the classes properly parse an objective from the AzD 
      ///  REST API
      /// </summary>
      [TestMethod]
      [DeploymentItem("SampleFiles/objective.json")]
      public void Parse_Objective()
      {
         // Arrange
         MemoryStream stream = LoadJSON("SampleFiles/objective.json");

         // Act
         var serializer = new DataContractJsonSerializer(typeof(WorkItem));
         var workItem = serializer.ReadObject(stream) as WorkItem;

         // Assert
         Assert.AreEqual(90, workItem.Id,
            "The work item id does not match");

         Assert.AreEqual("Objective", workItem.WorkItemType,
            "The work item type does not match.");
      }

      /// <summary>
      ///  Test that the classes properly parse a hook from the AzD 
      ///  web hook.
      /// </summary>
      [TestMethod]
      [DeploymentItem("SampleFiles/hook.json")]
      public void Parse_Hook()
      {
         // Arrange
         MemoryStream stream = LoadJSON("SampleFiles/hook.json");

         // Act
         var serializer = new DataContractJsonSerializer(typeof(Hook));
         var hook = serializer.ReadObject(stream) as Hook;

         // Assert
         Assert.AreEqual(91, hook.Resource.WorkItemId,
            "The work item id does not match");

         Assert.AreEqual("https://dev.azure.com/unittests/_apis/wit/workItems/90", hook.Resource.Revision.Relations[0].Url.ToString(),
            "The first related work item URL does not match");
      }


      /// <summary>
      /// Loads the sample files for unit testing parsing the JSON payloads.
      /// </summary>
      /// <param name="filename">Name of the sample file to load</param>
      /// <returns>Stream that can be used by serializer.</returns>
      private static MemoryStream LoadJSON(string filename)
      {
         var text = File.ReadAllText(filename);

         var stream = new MemoryStream();

         var writer = new StreamWriter(stream);
         writer.Write(text);
         writer.Flush();

         // We have to rewind the stream before we return it.
         stream.Position = 0;

         return stream;
      }
   }
}