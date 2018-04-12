namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

open Aardvark.Application

type Message = 
    | ToggleExpaneded of List<Index>
    | AddLeaf of List<Index>
    | AddNode of List<Index>
    | DeleteItem of List<Index>
    | Selected of List<Index>
    | Keydown of key : Keys
    | Keyup of key: Keys
    | StrgSelect of List<Index>
    | DragStart of List<Index>
    | DragStop of List<Index>*List<Index>
    | Nothing

[<DomainType>]
type LeafValue = 
    {
        name : string
        value : int
    }

[<DomainType>]
type NodeValue = 
    {
        name : string
        unit : string
    }

[<DomainType>]
type Properties = 
    { 
        isExpanded : bool
        isSelected : bool 
    }

[<DomainType>]
type LeafProperties = 
    { 
        isSelected : bool 
    }

[<DomainType>]
type Version =
    {
        version : int
    }

[<DomainType>]
type Tree =
    | Node of NodeValue * Properties * int * plist<Tree> 
    | Leaf of LeafValue * LeafProperties * int

[<AutoOpen; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Tree =
    let node v p i c = Node (v,p,i,c) //{ value = v; children = c; properties = p }
    let leaf v p i = Leaf (v,p,i)//{ value = v; properties = p }

[<DomainType>]
type TreeModel = 
    { 
        data: Tree 
        selected : plist<List<Index>>
        strgDown : bool
    }