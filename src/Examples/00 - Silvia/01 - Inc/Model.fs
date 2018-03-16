namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

type Message = 
    | Inc
    | NewName
    | Select of string

[<DomainType>]
type Model = 
    {
        value : int
        names : plist<string>
        selected : Option<string>
    }