namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

open Aardvark.Application

type Message = 
    | ToggleExpaneded of path : list<Index>
    | AddLeaf of path : list<Index>
    | AddNode of path : list<Index>
    | DeleteItem of path : list<Index>
    | Selected of path : list<Index>
    | Keydown of key : Keys
    | Keyup of key: Keys
    | DragStop of path : list<Index>
    | DragStart of path : list<Index>

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
    
//and [<DomainType>] NodeStuff =
//    { 
//        value: NodeValue
//        children: plist<Tree>
//        properties : Properties
//    }
//    
//and [<DomainType>] LeafStuff =
//    { 
//        value: LeafValue
//        properties : LeafProperties
//    }
[<AutoOpen; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Tree =
    let node v p c = Node (v,p,c) //{ value = v; children = c; properties = p }
    let leaf v p = Leaf (v,p)//{ value = v; properties = p }

[<DomainType>]
type TreeModel = 
    { 
        data: Tree 
        selected : plist<list<Index>>
        ctrlDown : bool
        drag : Option<list<Index>>
    }