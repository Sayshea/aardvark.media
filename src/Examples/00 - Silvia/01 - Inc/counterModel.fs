namespace counterModel

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

type Message =
    | IncCounter
    | DecCounter

[<DomainType>]
type Model =
    {
        counter : int
    }