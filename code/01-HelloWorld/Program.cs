namespace HelloWorld
{
  using Akka.Actor;

  internal class Program
  {
    private static void Main(string[] args)
    {
      // Create actor system
      ActorSystem actorSystem;
      // ActorSystem implements IDisposable and Dispose automatically
      // shuts the system down
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
    }
  }
}