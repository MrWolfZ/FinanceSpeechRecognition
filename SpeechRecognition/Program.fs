open Parsing
open Qif
open Speech
open System
open System.Speech.Recognition
open Models
open System.IO

[<EntryPoint>]
let main argv =
    let parse transactions result =
        match result with
        | Again -> 
            match transactions with
            | [] -> []
            | ts ->             
                printfn "Removed last transaction!"
                List.tail ts
        | Result r -> 
            let t = parseRecognitionResult r
            printfn "%s" <| writeEntry t
            t::transactions

    printfn """You can now start dictating transactions. Examples:
"spent 15 dollars at wall mart for entertainment 3 days ago"
"spent 6.42 at wall mart for groceries yesterday"
"spent 8 euros for dining out at restaurant on August 7th"

Say "Again" to remove the last entered transaction.
Say "Done" to finish dictation. A file containing all transactions will be written to your desktop.
"""
    
    let list = recognize parse []

    let entries = List.rev list |> List.map writeEntry
    let joined = String.Join("\r\n^\r\n", entries)
    let output = sprintf "!Type:Cash\r\n%s" joined

    let desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
    let filename = sprintf "%s.qif" <| DateTime.Now.ToString "yyyy-MM-dd HH-mm-ss-fff"
    File.WriteAllText(Path.Combine(desktop, filename), output)

    0
