namespace HelloWorld
{
  using System;

  using Akka.Actor;

  class HelloGuardianActor : TypedActor, IHandle<HelloWorldMessage>, IHandle<ShutdownMessage>
  {
    public void Handle(HelloWorldMessage message)
    {
      if (string.Compare("CREATE", message.Message,
        StringComparison.OrdinalIgnoreCase) == 0)
      {
        Context.ActorOf(Props.Create(() => new HelloActor()), "hello");
        return;
      }

      var child = Context.Child("hello");
      child.Tell(message);
    }

    public void Handle(ShutdownMessage message)
    {
      Context.System.Terminate();
    }
  }
}
