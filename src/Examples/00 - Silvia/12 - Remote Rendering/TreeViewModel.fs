namespace TreeViewModel

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open Aardvark.UI

[<DomainType>]
type TModel = {
    collapsed : hset<list<int>>
}

type InnerTree<'a> = InnerLeaf of 'a | InnerNode of 'a * list<InnerTree<'a>> 
