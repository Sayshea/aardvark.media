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
type Tree =
    | Node of value: NodeValue * properties : Properties * children: plist<Tree>
    | Leaf of value: LeafValue * properties : LeafProperties

[<AutoOpen; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Tree =
    let node v p c = Node(v, p, c)

[<DomainType>]
type TreeModel = 
    { 
        data: Tree 
        selected : plist<List<Index>>
        strgDown : bool
    }