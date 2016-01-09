namespace HelloWorld
{
  using System;
  using System.Diagnostics;

  using Akka.Actor;

  internal class HelloActor : ReceiveActor
  {
    private string lastPayload;

    public HelloActor()
    {
      FirstState();
    }

    public void FirstState()
    {
      Receive<HelloWorldMessage>(
        m =>
        {
          GenerateFailure(m);

          lastPayload = m.Message;
          BecomeStacked(SecondState);
        });

      Receive<ShutdownMessage>(
        m =>
        {
          Context.System.Shutdown();
        });
    }

    public void SecondState()
    {
      Receive<HelloWorldMessage>(
        m =>
        {
          GenerateFailure(m);

          Console.WriteLine("{0} {1}", lastPayload, m.Message);
          UnbecomeStacked();
        });
    }

    [Conditional("SUPERVISION")]
    private void GenerateFailure(HelloWorldMessage message)
    {
      if (message.Message.StartsWith("FAIL", StringComparison.OrdinalIgnoreCase))
      {
        Console.WriteLine("Simulating failure...");
        throw new Exception("Failed");
      }
    }
  }

  public class ShutdownMessage { }
}
