namespace DarqueWarrior.OKRGrader
{
   /// <summary>
   /// Class used to pass the results of grading an objective around.
   /// </summary>
   public class AverageResult
   {
      /// <summary>
      /// Number of key results used to calculate the average.
      /// </summary>
      public int Items { get; set; }

      /// <summary>
      /// The total of adding up the grades of each key result.
      /// </summary>
      public double Total { get; set; }

      /// <summary>
      /// The average of all the key result grades.
      /// </summary>
      public double Average { get; set; }

      /// <summary>
      /// The objective that was graded.
      /// </summary>
      public WorkItem Objective { get; set; }
   }
}
