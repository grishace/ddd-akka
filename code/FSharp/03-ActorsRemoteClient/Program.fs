open System
open Akka.FSharp
open Akka.Actor

open Messages

type ReadConsoleActor () =
  inherit Actor ()
  override this.OnReceive message =
    match message with
    | :? ConsoleMessage.ReadConsole ->
      let str = Console.ReadLine()
      match str.ToLowerInvariant() with
      | "exit" -> this.Sender <! ShutdownMessage ()
      | "cancel" -> this.Sender <! Calculation.Cancel ()
      | _ ->
        this.Sender <! ConsoleMessage.WriteConsole ((sprintf "You typed: %s" str), true)
        this.Sender <! Calculation.Start str
      Actor.Context.Stop(this.Self)
    | _ -> this.Unhandled ()

type WriteConsoleActor () =
  inherit Actor ()
  override this.OnReceive message =
    match message with
    | :? string as msg ->
      printfn "%s" msg
      Actor.Context.Stop(this.Self)
    | _ -> this.Unhandled ()

[<Literal>]
let remote = "akka.tcp://remote-server@remote-akka:8080/user/allocator"

type Coordinator () = 
  inherit Actor ()
  override this.OnReceive message =
     match message with
     | :? ConsoleMessage.ReadConsole as msg ->
       let read = Actor.Context.ActorOf(Props(typedefof<ReadConsoleActor>, Array.empty))
       read <! msg
     | :? ConsoleMessage.WriteConsole as msg ->
       let write = Actor.Context.ActorOf(Props(typedefof<WriteConsoleActor>, Array.empty))
       write <! msg.String
       if msg.Continue then this.Self <! ConsoleMessage.ReadConsole ()
     | :? Calculation.Result as msg ->
       let write = Actor.Context.ActorOf(Props(typedefof<WriteConsoleActor>, Array.empty))
       write <! sprintf "Compute from %s: %d" msg.String msg.Result
     | (:? Calculation.Start | :? Calculation.Cancel) as msg ->
         let alloc = Actor.Context.ActorSelection remote
         alloc <! msg
         match msg with
         | :? Calculation.Cancel -> this.Self <! ConsoleMessage.ReadConsole ()
         | _ -> ()
     | :? ShutdownMessage as msg ->
       let alloc = Actor.Context.ActorSelection remote
       alloc <! msg
       Actor.Context.System.Shutdown()
     | _ -> this.Unhandled ()

[<EntryPoint>]
let main argv = 
  let system = System.create "remote-client" <| Configuration.load()
  let coordinator = system.ActorOf<Coordinator> "coordinator"
  coordinator <! ConsoleMessage.ReadConsole ()
  system.AwaitTermination ()
  0
