namespace ActorsConsole
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

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

  public class CancelCalculation
  {
  }

  /// <summary>
  /// Do the same kind of CPU-bound task we started in AsyncConsole example.
  /// </summary>
  public class CalcActor : TypedActor, IHandle<StartCalculation>, IHandle<CancelCalculation>
  {
    private readonly CancellationTokenSource cancel = new CancellationTokenSource();

    public void Handle(StartCalculation message)
    {
      var context = Context;
      var sender = Sender;
      var self = Self;

      Task.Run(
        () =>
        {
          int size = 0;
          for (int z = 0; z < 100; z++)
          {
            for (int i = 0; i < 1000000; i++)
            {
              string value = i.ToString();
              size += value.Length;
              if (cancel.IsCancellationRequested)
              {
                throw new OperationCanceledException();
              }
            }
          }
          return size;
        }, cancel.Token)
        .ContinueWith(
          c =>
          {
            if (!(c.IsCanceled || c.IsFaulted))
            {
              // Send the calculation result back
              sender.Tell(new ResultMessage(message.Origin, c.Result));
            }

            // and stop the actor because it is not needed anymore
            context.Stop(self);
          }, TaskContinuationOptions.ExecuteSynchronously);
    }

    public void Handle(CancelCalculation message)
    {
      cancel.Cancel();
    }
  }
}