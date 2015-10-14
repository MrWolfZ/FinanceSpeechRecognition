module Parsing

open Models
open Speech
open System
open System.Speech.Recognition

let private parseCurrency str = 
    match str with
    | "EUR" -> EUR
    | "USD" -> USD
    | "CHF" -> CHF
    | _ -> failwithf "Unknown currency: %s" str

let private extractItem parser (semVal : SemanticValue) name = parser ((semVal.Item(name)).Value :?> string)

let private extractItemWithDefault parser (semVal : SemanticValue) name def = 
    if semVal.ContainsKey name then parser ((semVal.Item(name)).Value :?> string)
    else def

let parseRecognitionResult (msg : RecognitionResult) = 
    let values = msg.Semantics.Item "Values"
    let amount = extractItem Double.Parse values "Amount"
    let currency = extractItemWithDefault parseCurrency values "Currency" CHF
    let location = extractItem id values "Location"
    let category = extractItemWithDefault id values "Category" ""
    
    let date = 
        if values.ContainsKey "Date" then 
            let dateVal = values.Item "Date"
            let day = extractItem Int32.Parse dateVal "Day"
            let month = extractItem Int32.Parse dateVal "Month"
            let year = extractItemWithDefault Int32.Parse dateVal "Year" DateTime.Now.Year
            try 
                new DateTime(year, month, day)
            with _ -> 
                printfn "Failed to parse date. Returning today."
                DateTime.Today
        else 
            let relativeDate = extractItem Double.Parse values "RelativeDate"
            let today = DateTime.Today
            today.Add(TimeSpan.FromDays relativeDate)

    { 
        Amount = amount
        Currency = currency
        Date = date
        Payee = Payee location
        Category = Category category 
    }
