using Akka.Actor;

namespace ActorsConsole
{
    public class ShutdownMessage
    {
    }

    /// <summary>
    /// Responsible for handling all types of messages and resending them to the corresponding actors
    /// </summary>
    class CoordinatorActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is ReadConsoleMessage)
            {
                var read = Context.ActorOf(Props.Create(() => new ReadConsoleActor()));
                read.Tell(message);
                return;
            }

            if (message is WriteConsoleMessage)
            {
                var write = Context.ActorOf(Props.Create(() => new WriteConsoleActor()));
                var msg = (WriteConsoleMessage) message;
                write.Tell(msg.Message);

                // continue reading user input
                if (msg.ContinueRead)
                {
                    Self.Tell(new ReadConsoleMessage());
                }

                return;
            }

            // Resend calculation message
            if (message is StartCalculation)
            {
                // Allocate actor is not created every time - is created with the system and plays as mediator
                var alloc = Context.ActorSelection("/user/allocate");
                alloc.Tell(message);

                return;
            }

            // When receiving the result message extract payload and resend as write message
            if (message is ResultMessage)
            {
                var msg = (ResultMessage) message;
                Self.Tell(new WriteConsoleMessage(string.Format("Compute from {0}: {1:d}", msg.Origin, msg.Count), false));

                return;
            }

            // Handle shutdown
            if (message is ShutdownMessage)
            {
                Context.System.Shutdown();
                return;
            }

            Unhandled(message);
        }
    }
}
