namespace ActorsRemoteServer
{
  using Akka.Actor;

  internal class Program
  {
    private static void Main()
    {
      var actorSystem = ActorSystem.Create("ActorsRemoteServer");
      actorSystem.ActorOf(
        Props.Create(() => new AllocateRemoteActor()), "allocate");
      actorSystem.AwaitTermination();
    }
  }
}