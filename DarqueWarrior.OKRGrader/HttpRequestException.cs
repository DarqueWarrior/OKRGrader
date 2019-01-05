namespace DarqueWarrior.OKRGrader
{
   #region using
   using System;
   using System.Net;
   using System.Runtime.Serialization;
   using System.Security.Permissions;
   #endregion

   /// <summary>
   /// This exception is used to pass the status code to caller.
   /// </summary>
   [Serializable]
   public class HttpRequestException : Exception
   {
      /// <summary>
      /// The status code of the error response from the server.
      /// </summary>
      public HttpStatusCode StatusCode { get; set; }

      /// <summary>
      /// Initializes a new instance of the HttpRequestException class with a
      /// specified status code.
      /// </summary>
      /// <param name="statusCode">The status code of the response</param>
      public HttpRequestException(HttpStatusCode statusCode)
      {
         this.StatusCode = statusCode;
      }

      /// <summary>
      /// Initializes a new instance of the HttpRequestException class with a
      /// specified status code and error message.
      /// </summary>
      /// <param name="statusCode">The status code of the response</param>
      /// <param name="message">The message that describes the error</param>
      public HttpRequestException(HttpStatusCode statusCode, string message) : base(message)
      {
         this.StatusCode = statusCode;
      }

      /// <summary>
      /// Initializes a new instance of the HttpRequestException class with a
      /// specified status code, error message and a reference to the inner
      /// exception that is the cause of this exception.
      /// </summary>
      /// <param name="statusCode">The status code of the response</param>
      /// <param name="message">The message that describes the error</param>
      /// <param name="inner">The exception that is the cause of the current
      /// exception, or a null reference (Nothing in Visual Basic) if no inner
      /// exception is specified.</param>
      public HttpRequestException(HttpStatusCode statusCode, string message, Exception inner) : base(message, inner)
      {
         this.StatusCode = statusCode;
      }

      /// <summary>
      /// Constructor should be protected for unsealed classes, private for sealed classes.
      /// (The Serializer invokes this constructor through reflection, so it can be private)
      /// </summary>
      /// <param name="info">The System.Runtime.Serialization.SerializationInfo 
      /// that holds the serialized object data about the exception being thrown.</param>
      /// <param name="context">The System.Runtime.Serialization.StreamingContext
      /// that contains contextual information about the source or destination.</param>
      [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
      protected HttpRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
      {
      }

      /// <summary>
      /// Sets the System.Runtime.Serialization.SerializationInfo with 
      /// information about the exception.
      /// </summary>
      /// <param name="info">The System.Runtime.Serialization.SerializationInfo 
      /// that holds the serialized object data about the exception being thrown.</param>
      /// <param name="context">The System.Runtime.Serialization.StreamingContext
      /// that contains contextual information about the source or destination.</param>
      public override void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         if (info == null)
         {
            throw new ArgumentNullException("info");
         }

         info.AddValue("StatusCode", this.StatusCode);

         base.GetObjectData(info, context);
      }

      /// <summary>
      /// Compares two exceptions to determine if they have the same status
      /// code.
      /// </summary>
      /// <param name="obj">The other exception to compare with</param>
      /// <returns>true of the exceptions have the same status code</returns>
      public override bool Equals(object obj)
      {
         return obj is HttpRequestException exception &&
                StatusCode == exception.StatusCode;
      }

      /// <summary>
      /// Returns the hash of the status code of this exception.
      /// </summary>
      /// <returns>int hash code</returns>
      public override int GetHashCode()
      {
         return HashCode.Combine(StatusCode);
      }
   }
}