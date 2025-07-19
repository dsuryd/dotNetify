namespace DotNetify.Observer
{
   public class ObserverClientOptions
   {
      public bool UseMessagePack { get; set; }
      public int ConnectionPoolSize { get; set; } = 1;
   }
}