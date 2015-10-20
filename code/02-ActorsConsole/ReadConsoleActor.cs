using System;
using Akka.Actor;

namespace ActorsConsole
{
    public class ReadConsoleMessage
    {
    }

    public class ReadConsoleActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            // Stop the actor after the message is processed
            Context.Stop(Self);

            // Handle read message
            if (message is ReadConsoleMessage)
            {
                var str = Console.ReadLine();
                if (string.Compare("EXIT", str, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    // Send shutdown message when user has entered "exit"
                    Sender.Tell(new ShutdownMessage());
                    return;
                }

                // Send 2 messages: write message with the payload and start calculation
                Sender.Tell(new WriteConsoleMessage(string.Format("You typed: {0}", str)));
                Sender.Tell(new StartCalculation(str));

                return;
            }

            Unhandled(message);
        }
    }
}
