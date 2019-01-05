namespace DarqueWarrior.OKRGrader
{
   #region using
   using System;
   using System.Runtime.Serialization;
   #endregion

   /// <summary>
   /// Class that is the root of the relation JSON sent from AzD.
   /// This class is used to deserialize the request body.
   /// </summary>
   [DataContract(Name = "relation")]
   public class Relation
   {
      /// <summary>
      /// Holds how the work items are related. With this you can determine
      /// if this is a child or parent in the relationship. In AzD there are
      /// many ways to link work items and this lets you determine how they
      /// were linked.
      /// </summary>
      [DataMember(Name = "rel")]
      public string Rel { get; set; }

      /// <summary>
      /// This is the URL to the linked work item. Using this you can issue a 
      /// GET request against the AzD REST API to get the full work item back.
      /// </summary>
      [DataMember(Name = "url")]
      public Uri Url { get; set; }
   }
}
