namespace DDDAkka.HelloWorld
{
    using System.Runtime.Serialization;

    class HelloWorldMessage
    {
        [DataMember]
        public string Message { get; private set; }

        public HelloWorldMessage(string message)
        {
            this.Message = message;
        }
    }
}
