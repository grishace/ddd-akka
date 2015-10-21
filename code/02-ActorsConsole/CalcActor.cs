namespace ActorsConsole
{
  using Akka.Actor;

  public class ResultMessage
  {
    public string Origin { get; private set; }

    public int Count { get; private set; }

    public ResultMessage(string origin, int count)
    {
      Origin = origin;
      Count = count;
    }
  }

  /// <summary>
  /// Do the same kind of CPU-bound task we started in AsyncConsole example.
  /// </summary>
  public class CalcActor : TypedActor, IHandle<StartCalculation>
  {
    public void Handle(StartCalculation message)
    {
      int size = 0;
      for (int z = 0; z < 100; z++)
      {
        for (int i = 0; i < 1000000; i++)
        {
          string value = i.ToString();
          size += value.Length;
        }
      }

      // Send the calculation result back
      Sender.Tell(new ResultMessage(((StartCalculation)message).Origin, size));
      // and stop the actor because it is not needed anymore
      Context.Stop(Self);
    }
  }
}