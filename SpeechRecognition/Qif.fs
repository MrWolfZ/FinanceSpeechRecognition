module Qif

open System
open Models

type private Line =
    | D of DateTime
    | T of float
    | P of Payee
    | L of Category

let private parseLine (l: string) =
    let marker = l.Chars 0
    let value = l.Substring 1
    match marker with
    | 'D' -> Some <| D (DateTime.Parse value)
    | 'T' -> Some <| T (Double.Parse value)
    | 'P' -> Some <| P (Payee value)
    | 'L' -> Some <| L (Category value)
    | _ -> None

let readEntry (s: string) =
    let lines = s.Split('\n', '\r')

    let extendTransaction t l =
        match l with
        | D date -> { t with Date = date }
        | T amount -> { t with Amount = amount }
        | P payee -> { t with Payee = payee }
        | L category -> { t with Category = category }

    Array.filter (not << String.IsNullOrWhiteSpace) lines
    |> Array.map parseLine
    |> Array.filter Option.isSome
    |> Array.map Option.get
    |> Array.fold extendTransaction emptyTransaction

let writeEntry (t: Transaction) =
    let (Payee p) = t.Payee
    let (Category c) = t.Category
    sprintf "D%s\r\nT%.2f\r\nP%s\r\nL%s" (t.Date.ToString "yyyy-MM-dd") t.Amount p c
    
