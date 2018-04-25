namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open Aardvark.UI

open Aardvark.Application

type Message = 
    | ToggleExpaneded of list<Index>
    | Selected of list<Index>
    | Keydown of key : Keys
    | Keyup of key: Keys
    | StrgSelect of list<Index>
    | Nothing

[<DomainType>]
type LeafValue = 
    {
        printMessage : DomNode<Message>
    }

[<DomainType>]
type NodeValue = 
    {
        printMessage : DomNode<Message>
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
    }