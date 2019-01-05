namespace DarqueWarrior.OKRGrader
{
   #region using
   using System.Runtime.Serialization;
   #endregion

   /// <summary>
   /// Class that is the root of the hook JSON sent from AzD.
   /// This class is used to deserialize the request body.
   /// </summary>
   [DataContract(Name = "hook")]
   public class Hook
   {
      /// <summary>
      /// The only portion of the hook we need is the resource.
      /// It holds the URL to the objective work item we need to find all the
      /// link key results.
      /// </summary>
      [DataMember(Name = "resource")]
      public Resource Resource { get; set; } 
   }
}
