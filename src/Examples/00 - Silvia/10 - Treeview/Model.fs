namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

open Aardvark.Application

type Message = 
    | ToggleExpaneded of list<Index>
    | AddLeaf of list<Index>
    | AddNode of list<Index>
    | DeleteItem of list<Index>
    | Selected of list<Index>
    | Keydown of key : Keys
    | Keyup of key: Keys
    | StrgSelect of list<Index>
    | DragStop of list<Index>
    | DragStart of list<Index>
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
type Tree =
    | Node of NodeValue * Properties * plist<Tree>
    | Leaf of LeafValue * LeafProperties
    
and [<DomainType>] NodeStuff =
    { 
        value: NodeValue
        children: plist<Tree>
        properties : Properties
    }
    
and [<DomainType>] LeafStuff =
    { 
        value: LeafValue
        properties : LeafProperties
    }
[<AutoOpen; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Tree =
    let node v p c = Node (v,p,c) //{ value = v; children = c; properties = p }
    let leaf v p = Leaf (v,p)//{ value = v; properties = p }

[<DomainType>]
type TreeModel = 
    { 
        data: Tree 
        selected : plist<list<Index>>
        strgDown : bool
        drag : Option<list<Index>>
    }