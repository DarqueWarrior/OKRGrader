namespace DarqueWarrior.OKRGrader
{
   #region using
   using System.Collections.Generic;
   using System.Runtime.Serialization;
   #endregion

   /// <summary>
   /// Class that is the root of the revision JSON sent from AzD.
   /// This class is used to deserialize the request body.
   /// 
   /// This class holds the all important relations. The relations are were
   /// we get all the related work items. We will use this to find all the
   /// linked key results to grade the objective.
   /// </summary>
   [DataContract(Name = "revision")]
   public class Revision
   {
      /// <summary>
      /// List of related work items.
      /// </summary>
      [DataMember(Name = "relations")]
      public List<Relation> Relations { get; set; }
   }
}