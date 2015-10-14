module Models

open System

type Currency =
    | EUR
    | CHF
    | USD

type Payee = Payee of string
type Category = Category of string

type Transaction = {
    Date: DateTime
    Amount: float
    Currency: Currency
    Payee: Payee
    Category: Category
}

let emptyTransaction = {
    Date = DateTime.Now
    Amount = 0.0
    Currency = CHF
    Payee = Payee ""
    Category = Category ""
}