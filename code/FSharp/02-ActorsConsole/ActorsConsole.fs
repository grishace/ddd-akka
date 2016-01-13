open System
open Akka.FSharp
open Akka.Actor

type ConsoleMessage = 
| ReadConsole
| WriteConsole of (string * bool)

type Calculation =
| Start of string
| Result of string * int
| Cancel

type ShutdownMessage = ShutdownMessage

type ReadConsoleActor () =
  inherit Actor ()
  override this.OnReceive message =
    match message with
    | :? ConsoleMessage as msg ->
      match msg with
      | ReadConsole ->
        let str = Console.ReadLine()
        match str.ToLowerInvariant() with
        | "exit" -> this.Sender <! ShutdownMessage 
        | "cancel" -> this.Sender <! Cancel
        | _ ->
          this.Sender <! WriteConsole ((sprintf "You typed: %s" str), true)
          this.Sender <! Start str
        Actor.Context.Stop(this.Self)
      | _ -> this.Unhandled ()
    | _ -> this.Unhandled ()

type WriteConsoleActor () =
  inherit Actor ()
  override this.OnReceive message =
    match message with
    | :? string as msg ->
      printfn "%s" msg
      Actor.Context.Stop(this.Self)
    | _ -> this.Unhandled ()

type Coordinator () = 
  inherit Actor ()
  override this.OnReceive message =
     match message with
     | :? ConsoleMessage as msg ->
       match msg with
       | ReadConsole ->
        let read = Actor.Context.ActorOf(Props(typedefof<ReadConsoleActor>, Array.empty))
        read <! msg
       | WriteConsole (m, b) ->
        let write = Actor.Context.ActorOf(Props(typedefof<WriteConsoleActor>, Array.empty))
        write <! m
        if b then this.Self <! ReadConsole
     | :? Calculation as msg ->
       match msg with 
       | Result (s, i) ->
         let write = Actor.Context.ActorOf(Props(typedefof<WriteConsoleActor>, Array.empty))
         write <! sprintf "Compute from %s: %d" s i
       | _ ->
         let alloc = Actor.Context.ActorSelection "/user/allocator"
         alloc <! msg
         match msg with
         | Cancel -> this.Self <! ReadConsole
         | _ -> ()
     | :? ShutdownMessage -> Actor.Context.System.Shutdown()
     | _ -> this.Unhandled ()

open System.Threading
open System.Threading.Tasks

type CalculationActor () =
  inherit Actor()

  // Note: must be a ref, can't request cancellation otherwise
  let ctsRef = ref <| new CancellationTokenSource()

  override this.OnReceive message =
    match message with
    | :? Calculation as msg ->
      match msg with
      | Start s -> 
        let sender = this.Sender
        let self = this.Self
        let ctx = Actor.Context

        let task = 
          async {
            let mutable size = 0
            for z in { 0..99 } do
              for i in { 0..999999 } do
                let s = string i
                size <- size + s.Length
            return size
          }
        
        let run =
          async {
            let! res = Async.StartAsTask(task, TaskCreationOptions.None, (!ctsRef).Token) |> Async.AwaitTask
            // reply result back to sender if not cancelled
            if not((!ctsRef).IsCancellationRequested) then sender <! Result (s, res)
            (!ctsRef).Dispose ()
            ctx.Stop(self)
          }

        Async.Start run
      | Cancel -> (!ctsRef).Cancel()
      | _ -> this.Unhandled ()
    | _ -> this.Unhandled ()

type Allocator () =
  inherit Actor () 
  override this.OnReceive message =
    match message with
    | :? Calculation as msg ->
      match msg with
      | Start _ ->
        // use different dispatcher so it won't affect (mostly) the akka system itself
        let calc = Actor.Context.ActorOf(Props.Create<CalculationActor>().WithDispatcher("akka.actor.calc-dispatcher"))
        calc.Forward msg
      | Cancel -> 
        let calc = Actor.Context.ActorSelection "/user/allocator/*"  
        calc <! msg
      | _ -> this.Unhandled ()
    | _ -> this.Unhandled ()
     
[<EntryPoint>]
let main argv = 
  let system = System.create "dotnetperls" <| Configuration.load()
  system.ActorOf<Allocator> "allocator" |> ignore
  let coordinator = system.ActorOf<Coordinator> "coordinator"
  coordinator <! ReadConsole
  system.AwaitTermination ()
  0
