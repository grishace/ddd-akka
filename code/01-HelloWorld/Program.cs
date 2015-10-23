namespace HelloWorld
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

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
      helloActor.Tell(new HelloWorldMessage("And "));
      // bacause of behavior change the following message will be skipped
      helloActor.Tell(new ShutdownMessage());
      // and this one will be added to the previous message and printed
      helloActor.Tell(new HelloWorldMessage("Denver Dev Day!"));
      helloActor.Tell(new ShutdownMessage());
      actorSystem.AwaitTermination();

      #endregion

      #region with supervising strategy

//      actorSystem = ActorSystem.Create("HelloWorld");
//      var guardian = actorSystem.ActorOf(Props.Create(() => new HelloGuardianActor()).WithSupervisorStrategy(new OneForOneStrategy(1, -1,
//        e =>
//        {
//          Console.WriteLine("Restarting?");
//          return Directive.Restart;
//        })), "guardian");
//      // create child actor
//      guardian.Tell(new HelloWorldMessage("Create"));
//      guardian.Tell(new HelloWorldMessage("1..."));
//      guardian.Tell(new HelloWorldMessage("2..."));
//      // simulate failure
//      guardian.Tell(new HelloWorldMessage("Fail"));
//      // next two messages should work as if actor was created and did not fail
//      guardian.Tell(new HelloWorldMessage("Actor has been restarted"));
//      guardian.Tell(new HelloWorldMessage("and continue processing messages"));
//
//      Console.ReadLine();
//
//      guardian.Tell(new ShutdownMessage());
//      actorSystem.AwaitTermination();

      #endregion
    }
  }
}