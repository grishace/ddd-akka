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
      helloActor.Tell(new ShutdownMessage());
      helloActor.Tell(new HelloWorldMessage("Denver Dev Day!"));
      helloActor.Tell(new ShutdownMessage());
      actorSystem.AwaitTermination();

      #endregion
    }
  }
}