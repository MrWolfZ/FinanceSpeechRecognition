module Speech

open System.Speech.Recognition
open System.Speech.Recognition.SrgsGrammar

let private sr = new SpeechRecognitionEngine()
sr.SetInputToDefaultAudioDevice()

let private doc = new SrgsDocument("patterns.grxml")

let private g = new Grammar(doc)
sr.LoadGrammar g

type Result =
    | Result of RecognitionResult
    | Again

let private (|Result|Again|Exit|) (result: RecognitionResult) =
   if result.Semantics.ContainsKey "Again" then Again
   else if result.Semantics.ContainsKey "Exit" then Exit
   else Result result   

let recognize fn initial = 
    let rec loop state = 
        let result = sr.Recognize()
        match result with
        | null -> 
            printfn "Received null as speech recognition result!"
            loop state
        | _ -> 
            match result with
            | Result r -> loop <| fn state (Result r)
            | Again -> loop <| fn state Again
            | Exit -> state
    loop initial
