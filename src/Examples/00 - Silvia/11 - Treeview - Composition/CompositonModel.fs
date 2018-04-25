namespace CompositionModel

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open Aardvark.UI

open Model

[<DomainType>]
type TModel<'a> = {
    tree : hset<'a>
}

//    type Model<'a> = {
//        expandedNodes : hset<'a>
//        selected : Option<'a>
//    }