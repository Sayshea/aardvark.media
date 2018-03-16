namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

type pos = int * int

type Message = 
    | Flip of pos

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
    }