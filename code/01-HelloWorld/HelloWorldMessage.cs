namespace HelloWorld
{
  internal class HelloWorldMessage
  {
    /// <summary>
    /// Message payload.
    /// </summary>
    /// <remarks>
    /// Private setter - immutable!
    /// </remarks>
    public string Message { get; private set; }

    public HelloWorldMessage(string message)
    {
      Message = message;
    }
  }
}