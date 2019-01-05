namespace DarqueWarrior.OKRGrader
{
   #region using
   using System.Runtime.Serialization;
   #endregion

   /// <summary>
   /// Class that is the root of the key result JSON sent from AzD.
   /// This class is used to deserialize the request body.
   /// </summary>
   [DataContract(Name = "keyresult")]
   public class KeyResult : WorkItem
   {
      /// <summary>
      /// Not all the linked items to an objective have to be key results. 
      /// Users might link tasks or other work item types as well. You can 
      /// use this constant to test the work item type to make sure it is a
      /// key result.
      /// </summary>
      public const string WorkItemTypeValue = "Key Result";

      /// <summary>
      /// Gets the grade for the key result. 
      /// </summary>
      public double Grade { get { return this.Fields.Grade; } }
   }
}
