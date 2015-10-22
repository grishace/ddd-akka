namespace HelloWorld
{
  using Akka.Actor;

  internal class Program
  {
    private static void Main(string[] args)
    {
      // Create actor system
      ActorSystem actorSystem;
      using (actorSystem = ActorSystem.Create("HelloWorld"))
      {
        // Create actor
        var helloWorldActor =
          actorSystem.ActorOf(Props.Create(() => new HelloWorldActor()));
        // Send the message
        helloWorldActor.Tell(new HelloWorldMessage("Hello, World!"));
      }
      // Wait for the system graceful shutdown
      actorSystem.AwaitTermination();

      #region with behavior change

      actorSystem = ActorSystem.Create("HelloWorld");
      var helloActor = actorSystem.ActorOf(Props.Create(() => new HelloActor()));
      helloActor.Tell(new HelloWorldMessage("\nAnd "));
      // bacause of behavior change the following message will be skipped
      helloActor.Tell(new ShutdownMessage());
      // and this one will be added to the previous message and printed
      helloActor.Tell(new HelloWorldMessage("Denver Dev Day!"));
      helloActor.Tell(new ShutdownMessage());
      actorSystem.AwaitTermination();

      #endregion
    }
  }
}