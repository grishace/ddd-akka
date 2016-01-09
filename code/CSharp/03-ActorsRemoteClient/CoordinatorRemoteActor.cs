namespace ActorsRemoteClient
{
  using ActorsConsole;
  using Akka.Actor;

  internal class CoordinatorRemoteActor : UntypedActor
  {
    protected override void OnReceive(object message)
    {
      if (message is ReadConsoleMessage)
      {
        var read = Context.ActorOf(Props.Create(() => new ReadConsoleActor()));
        read.Tell(message);
        return;
      }

      if (message is WriteConsoleMessage)
      {
        var write = Context.ActorOf(Props.Create(() => new WriteConsoleActor()));
        var msg = (WriteConsoleMessage)message;
        write.Tell(msg.Message);

        if (msg.ContinueRead)
        {
          Self.Tell(new ReadConsoleMessage());
        }

        return;
      }

      // This is the only difference (besides the configuration) - select remote actor
      var isCancel = message is CancelCalculation;
      if ((message is StartCalculation) || isCancel)
      {
        // Allocate actor is not created every time - is created with the system and plays as mediator
        var alloc =
          Context.ActorSelection(
            "akka.tcp://ActorsRemoteServer@remote-akka:8080/user/allocate");
        alloc.Tell(message);

        // continue normal operation
        if (isCancel)
        {
          Self.Tell(new ReadConsoleMessage());
        }
      }

      if (message is ResultMessage)
      {
        var msg = (ResultMessage)message;
        Self.Tell(
          new WriteConsoleMessage(
            string.Format("Compute from {0}: {1:d}", msg.Origin, msg.Count),
            false));

        return;
      }

      if (message is ShutdownMessage)
      {
        var alloc =
          Context.ActorSelection(
            "akka.tcp://ActorsRemoteServer@remote-akka:8080/user/allocate");
        alloc.Tell(message);

        Context.System.Shutdown();

        return;
      }

      Unhandled(message);
    }
  }
}