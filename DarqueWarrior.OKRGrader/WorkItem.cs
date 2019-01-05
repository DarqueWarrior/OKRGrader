namespace DarqueWarrior.OKRGrader
{
   #region using
   using System;
   using System.Collections.Generic;
   using System.Runtime.Serialization;
   #endregion

   /// <summary>
   /// The base class for parsing a work item returned using the AzD REST API.
   /// </summary>
   [DataContract(Name = "WorkItem")]
   public class WorkItem
   {
      /// <summary>
      /// The collection of related work items.
      /// </summary>
      [DataMember(Name = "relations")]
      public List<Relation> Relations { get; set; }

      /// <summary>
      /// The unique ID of this work item in AzD
      /// </summary>
      [DataMember(Name = "id")]
      public int Id { get; set; }

      /// <summary>
      /// The URL that is the base of REST API calls against this work item.
      /// </summary>
      [DataMember(Name = "url")]
      public Uri Url { get; set; }

      /// <summary>
      /// There is no need to expose the fields to consumers of these classes.
      /// That is why it is defined inside this class and protected.
      /// Custom types like Grade are stored inside the fields object of the
      /// JSON.
      /// </summary>
      [DataContract(Name = "fields")]
      protected class Values
      {
         /// <summary>
         /// Identifies the type of work item in the JSON. This is used to
         /// make sure that only the key results linked to the objective
         /// are counted.
         /// </summary>
         [DataMember(Name = "System.WorkItemType")]
         public string WorkItemType { get; set; }

         /// <summary>
         /// Only on Key Results
         /// This is the grade the user assigned to the key result. This will
         /// be used to calculate the average on the objective.
         /// </summary>
         [DataMember(Name = "Custom.Grade")]
         public double Grade { get; set; }
      }

      /// <summary>
      /// A series of name value pairs of the fields of a work item.
      /// </summary>
      [DataMember(Name = "fields")]
      protected Values Fields { get; set; }

      /// <summary>
      /// A convenience property that surfaces the work item type of fields.
      /// This just makes this data easier to use.
      /// </summary>
      public string WorkItemType { get { return this.Fields.WorkItemType; } }
   }
}
