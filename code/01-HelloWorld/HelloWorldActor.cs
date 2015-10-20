namespace HelloWorld
{
    using System;
    using Akka.Actor;

    /// <summary>
    /// Actor to handle <see cref="HelloWorldMessage"/>
    /// </summary>
    class HelloWorldActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is HelloWorldMessage)
            {
                // extract message string
                var msg = message as HelloWorldMessage;
                Console.WriteLine(msg.Message);
                return;
            }

            Unhandled(message);
        }
    }
}
