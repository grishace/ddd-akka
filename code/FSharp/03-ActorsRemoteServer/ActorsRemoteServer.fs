open Akka.FSharp
open Akka.Actor

open System.Threading
open System.Threading.Tasks

open Messages

type CalculationActor () =
  inherit Actor()

  // Note: must be a ref, can't request cancellation otherwise
  let ctsRef = ref <| new CancellationTokenSource()

  override this.OnReceive message =
    match message with
    | :? Calculation.Start as msg -> 
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
          let! res = 
            Async.StartAsTask(task, TaskCreationOptions.None, (!ctsRef).Token)
            |> Async.AwaitTask
          // reply result back to sender if not cancelled
          if not((!ctsRef).IsCancellationRequested) then 
            sender <! Calculation.Result (msg.String, res)
          (!ctsRef).Dispose ()
          ctx.Stop(self)
        }
     
      Async.Start run
    | :? Calculation.Cancel -> (!ctsRef).Cancel()
    | _ -> this.Unhandled ()

type Allocator () =
  inherit Actor () 
  override this.OnReceive message =
    match message with
    | :? Calculation.Start as msg ->
      let calc = Actor.Context.ActorOf(Props(typedefof<CalculationActor>, Array.empty))
      calc.Forward(msg)
    | :? Calculation.Cancel as msg -> 
      let calc = Actor.Context.ActorSelection "/user/allocator/*"  
      calc <! msg
    | :? ShutdownMessage -> Actor.Context.System.Shutdown ()
    | _ -> this.Unhandled ()


[<EntryPoint>]
let main argv = 
  let system = System.create "remote-server" <| Configuration.load()
  system.ActorOf<Allocator> "allocator" |> ignore
  system.AwaitTermination ()
  0
