namespace ActorsRemoteClient
{
    using ActorsConsole;
    using Akka.Actor;

    class Program
    {
        static void Main()
        {
            var actorSystem = ActorSystem.Create("ActorsRemoteClient");
            var coordinator = actorSystem.ActorOf(Props.Create(() => new CoordinatorRemoteActor()));
            coordinator.Tell(new ReadConsoleMessage());

            actorSystem.AwaitTermination();
        }
    }
}
