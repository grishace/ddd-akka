open Akka.FSharp
open Akka.Actor
open System.Threading

let run example = 
  let awaitTermination (system:ActorSystem) =
    Thread.Sleep(100)    
    system.Shutdown()  
    system.AwaitTermination()
  example() |> awaitTermination 

let helloWorld () = 
  let system = System.create "helloworld-system" (Configuration.load())
  let actor = spawn system "helloworld-actor" (actorOf (fun m -> printfn "%s" m))
  actor <! "Hello, World!"
  system

type UserMessage =
| Message of string
| Test
  
#if BEHAVIOR

let secondState lastPayload (mailbox:Actor<_>) (message:obj) =
  match message with
  | :? UserMessage as m ->
    match m with
    | Message s -> 
        printfn "%s %s" lastPayload s
        mailbox.Context.UnbecomeStacked()
        true
    | _ -> false
  | _ -> false

let firstState (mailbox:Actor<_>) message =
   match message with
   | Message s -> mailbox.Context.BecomeStacked(Receive (secondState s mailbox))
   | Test -> printfn "---"

let withBehaviorChange () =
  let system = System.create "behavior-system" (Configuration.load())
  let actor = spawn system "beahvior-actor" (actorOf2 firstState)

  actor <! Test
  actor <! Message("And")
  actor <! Test
  actor <! Message("Golden Func Meetup!")
  actor <! Test
 
  system

#endif

#if SUPERVISION

type SupervisionMessage =
| Create
| Message of string
| Fail

type ChildActor() =
  inherit Actor()
  override this.OnReceive message =
    match message with
    | :? SupervisionMessage as msg ->
      match msg with
      | Message m -> printfn "%s" m
      | Fail -> failwith "Failure"
      | _ -> this.Unhandled()
    | _ -> this.Unhandled()

let guardianReceive (mailbox:Actor<_>) message =
  match message with
  | Create -> mailbox.ActorOf(Props(typedefof<ChildActor>, Array.empty), "child") |> ignore
  | msg -> mailbox.Context.Child("child") <! msg

let guardianStrategy error =
  match error with
  | _ -> Directive.Restart

let withSupervision () =
  let system = System.create "supervision-system" (Configuration.load())
  let guardian = spawnOpt system "guardian-actor" (actorOf2 guardianReceive)
                   [ SpawnOption.SupervisorStrategy (Strategy.OneForOne(guardianStrategy, 1, Timeout.InfiniteTimeSpan)) ]

  guardian <! Create
  guardian <! Message "1..."
  guardian <! Message "2..."
  // simulate failure
  guardian <! Fail
  // next two messages should work as if actor was created and did not fail
  guardian <! Message "Actor has been restarted"
  guardian <! Message "and continue processing messages"

  system

#endif

[<EntryPoint>]
let main argv = 

  run helloWorld

#if BEHAVIOR
  run withBehaviorChange
#endif

#if SUPERVISION
  run withSupervision
#endif

  0
