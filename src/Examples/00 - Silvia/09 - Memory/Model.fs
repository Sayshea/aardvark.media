namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

open System

type pos = int * int

type Message = 
    | Flip of pos
    | Reset

type card =
    {
        value : int
        flipped : bool
    }

[<DomainType>]
type Model = 
    {
        numberFlipped : int
        board : hmap<pos,card>
        firstFlipped : pos
        secondFlipped : pos
        moves : int
        timer : DateTime
        infoText : string
    }