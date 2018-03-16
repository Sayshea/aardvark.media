namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

open System

type Message =
    | Tick

type Times =
    | Hour of float
    | Minute of float
    | Second of float

type Time = {
    Hours : float
    Minutes : float
    Seconds : float
}

[<DomainType>]
type Model = {
    currentTime : Time
}
