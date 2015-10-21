namespace ActorsConsole
{
  using Akka.Actor;

  public class StartCalculation
  {
    public string Origin { get; private set; }

    public StartCalculation(string origin)
    {
      Origin = origin;
    }
  }

  /// <summary>
  /// Parent of calculation actors
  /// </summary>
  /// <remarks>
  /// Due to the nature of the calculation its dispatcher is configured separately
  /// </remarks>
  internal class AllocateActor : UntypedActor
  {
    protected override void OnReceive(object message)
    {
      // handle calculation message
      if (message is StartCalculation)
      {
        var calc =
          Context.ActorOf(
            Props.Create<CalcActor>()
              .WithDispatcher("actor.akka.calc-dispatcher"));
        calc.Forward(message);
        return;
      }

      // handle shutdown message
      if (message is ShutdownMessage)
      {
        Context.System.Shutdown();
        return;
      }

      Unhandled(message);
    }
  }
}