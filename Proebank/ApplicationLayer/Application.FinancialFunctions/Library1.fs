namespace Application.FinancialFunctions
open System
open System.Collections
open System.Collections.Generic
open System.Linq
open Domain.Models.Accounts
open Domain.Models.Loans

module Interest = 
    let TotalSum (tariff: Tariff, sum: decimal, term: int) =
        sum + sum * ((decimal)term * tariff.InterestRate / 12M)
        