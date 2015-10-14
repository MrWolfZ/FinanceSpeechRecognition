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
            printfn "Again"
            List.tail transactions
        | Result r -> 
            let t = parseRecognitionResult r
            printfn "%s" <| writeEntry t
            t::transactions

    let list = recognize parse []

    let entries = List.rev list |> List.map writeEntry
    let joined = String.Join("\r\n^\r\n", entries)
    let output = sprintf "!Type:Cash\r\n%s" joined

    let desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
    let filename = sprintf "%s.qif" <| DateTime.Now.ToString "yyyy-MM-dd HH-mm-ss-fff"
    File.WriteAllText(Path.Combine(desktop, filename), output)

    0
