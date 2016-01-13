namespace Messages

type ShutdownMessage () =
  class
  end

module ConsoleMessage =

  type ReadConsole () =
    class
    end

  type WriteConsole (s:string, b:bool) =
    class
      member val String = s with get, set
      member val Continue = b with get, set
    end

module Calculation = 

  type Start (s:string) =
    class
      member val String = s with get, set
    end

  type Result(s:string, r:int) =
    class
      member val String = s with get, set
      member val Result = r with get, set
    end

  type Cancel () =
    class
    end