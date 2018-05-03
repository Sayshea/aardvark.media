namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open Aardvark.UI

open TreeViewModel

type UserTree = UserLeaf of string | UserNode of string * list<UserTree>

[<DomainType>]
type Model = {
    [<TreatAsValue>] // we want mod<tree> not mod<usertree>
    cTree : UserTree

    treeModel : TreeViewModel.TModel
}
