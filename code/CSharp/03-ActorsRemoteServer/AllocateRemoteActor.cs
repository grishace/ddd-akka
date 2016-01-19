namespace ActorsRemoteServer
{
  using ActorsConsole;
  using Akka.Actor;

  internal class AllocateRemoteActor : UntypedActor
  {
    protected override void OnReceive(object message)
    {
      if (message is StartCalculation)
      {
        var calc = Context.ActorOf(Props.Create(() => new CalcActor()));
        calc.Forward(message);
        return;
      }

      if (message is CancelCalculation)
      {
        var calc = Context.ActorSelection("/user/allocate/*");
        calc.Tell(message);
      }

      // Handle additional shutdown message
      if (message is ShutdownMessage)
      {
        Context.System.Terminate();
        return;
      }

      Unhandled(message);
    }
  }
}