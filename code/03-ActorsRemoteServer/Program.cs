namespace ActorsRemoteServer
{
    using Akka.Actor;

    class Program
    {
        static void Main()
        {
            var actorSystem = ActorSystem.Create("ActorsRemoteServer");
            actorSystem.ActorOf(Props.Create(() => new AllocateRemoteActor()), "allocate");
            actorSystem.AwaitTermination();
        }
    }
}
