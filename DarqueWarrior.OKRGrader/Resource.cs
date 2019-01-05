namespace DarqueWarrior.OKRGrader
{
   #region using
   using System.Runtime.Serialization;
   #endregion

   /// <summary>
   /// Class that is the root of the resource JSON sent from AzD.
   /// This class is used to deserialize the request body.
   /// </summary>
   [DataContract(Name = "resource")]
   public class Resource
   {
      /// <summary>
      /// The id of the work item sent in the hook.
      /// </summary>
      [DataMember(Name = "workItemId")]
      public int WorkItemId { get; set; }

      /// <summary>
      /// This holds the all important relations. The relations are were
      /// we get all the related work items. We will use this to find all the
      /// linked key results to grade the objective.
      /// </summary>
      [DataMember(Name = "revision")]
      public Revision Revision { get; set; }
   }
}