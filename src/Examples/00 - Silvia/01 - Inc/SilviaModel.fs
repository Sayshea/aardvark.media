namespace SilviaModel

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

type SMessage =
    | SetInput of string
    | IncCounter
    | DecCounter
    | SetVariableInput of string * string
    | AddToInputList of string

[<DomainType>]
type SModel =
    {
        inputString : string
        counter : int
        inputString1 : string
        inputString2 : string
        inputString3 : string
        inputStringUnteres : string
        inputList : plist<string>
    }