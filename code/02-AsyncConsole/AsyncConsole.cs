using System;
using System.Threading.Tasks;

/// <summary>
/// Very simple demonstration of the async task. 
/// </summary>
/// <remarks>
/// Slightly modified version of http://www.dotnetperls.com/async
/// </remarks>
internal class AsyncConsole
{
  private static void Main()
  {
    while (true)
    {
      // Handle user input
      string result = Console.ReadLine();
      Console.WriteLine("You typed: {0}", result);
      // Start computation.
      Example(result);
    }
  }

  private static async void Example(string origin)
  {
    // This method runs asynchronously.
    int t = await Task.Run(() => Allocate());
    Console.WriteLine("Compute {0}: {1}", origin, t);
  }

  private static int Allocate()
  {
    // Compute total count of digits in strings
    int size = 0;
    for (int z = 0; z < 100; z++)
    {
      for (int i = 0; i < 1000000; i++)
      {
        string value = i.ToString();
        size += value.Length;
      }
    }
    return size;
  }
}