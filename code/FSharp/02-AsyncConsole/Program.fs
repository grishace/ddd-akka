// Async computation example
// from http://www.dotnetperls.com/async
// implemented in F#

let allocate () = 
    let mutable size = 0
    for z in { 0..99 } do
        for i in { 0..999999 } do
            let s = string i
            size <- size + s.Length
    size

let example str = async {
        let! t = async {
            return allocate ()
        }
        printf "Compute %s: %d\n" str t
    }

let rec run start str =
    if start
      then (example str) |> Async.Start
    let r = System.Console.ReadLine()
    printf "You typed: %s\n" r
    run true r

[<EntryPoint>]
let main argv = 
    run false ""