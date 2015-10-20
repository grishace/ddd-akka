namespace ActorsRemoteServer
{
    using ActorsConsole;
    using Akka.Actor;

    class AllocateRemoteActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is StartCalculation)
            {
                var calc = Context.ActorOf(Props.Create(() => new CalcActor()));
                calc.Forward(message);
                return;
            }

            // Handle additional shutdown message
            if (message is ShutdownMessage)
            {
                Context.System.Shutdown();
                return;
            }

            Unhandled(message);
        }
    }
}
