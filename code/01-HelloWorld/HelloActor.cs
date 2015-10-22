namespace HelloWorld
{
  using System;
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
          Console.WriteLine("{0} {1}", lastPayload, m.Message);
          UnbecomeStacked();
        });
    }
  }

  public class ShutdownMessage { }
}
