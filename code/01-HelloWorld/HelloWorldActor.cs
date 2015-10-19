namespace DDDAkka.HelloWorld
{
    using System;
    using Akka.Actor;

    class HelloWorldActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is HelloWorldMessage)
            {
                var msg = message as HelloWorldMessage;
                Console.WriteLine(msg.Message);
                return;
            }

            this.Unhandled(message);
        }
    }
}
