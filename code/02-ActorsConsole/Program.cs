namespace ActorsConsole
{
    using Akka.Actor;

    /// <summary>
    /// Actor-based version of AsyncConsole project.
    /// </summary>
    class Program
    {
        static void Main()
        {
            // Create actor system
            var actorSystem = ActorSystem.Create("DotNetPerlsAsync");
            // Separate CPU-bound actor
            actorSystem.ActorOf(Props.Create<AllocateActor>().WithDispatcher("akka.actor.calc-dispatcher"), "allocate");

            // Create coordinating actor and send initial message
            var coordinator = actorSystem.ActorOf(Props.Create(() => new CoordinatorActor()));
            coordinator.Tell(new ReadConsoleMessage());

            // Block main execution thread until actor system is shut down
            actorSystem.AwaitTermination();
        }
    }
}
