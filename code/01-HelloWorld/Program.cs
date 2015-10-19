namespace DDDAkka.HelloWorld
{
    using System;
    using Akka.Actor;

    class Program
    {
        static void Main(string[] args)
        {
            ActorSystem actorSystem;
            using (actorSystem = ActorSystem.Create("HelloWorld"))
            {
                var helloWorldActor = actorSystem.ActorOf(Props.Create(() => new HelloWorldActor()));
                helloWorldActor.Tell(new HelloWorldMessage("Hello, World!"));
                Console.ReadLine();
            }

            actorSystem.AwaitTermination();
        }
    }
}
