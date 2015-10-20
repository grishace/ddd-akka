using System;
using Akka.Actor;

namespace ActorsConsole
{
    public class WriteConsoleMessage
    {
        public string Message { get; private set; }
        public bool ContinueRead { get; private set; }

        public WriteConsoleMessage(string message, bool continueRead = true)
        {
            Message = message;
            ContinueRead = continueRead;
        }
    }

    public class WriteConsoleActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            // Stop the actor after the message is processed
            Context.Stop(Self);

            if (message is string)
            {
                // write message payload to the console
                Console.WriteLine(message);
                return;
            }

            Unhandled(message);
        }
    }
}
