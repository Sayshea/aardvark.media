module App

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Aardvark.Application

open Model

open System

let pvalue (value : string) =
    text value

let defaultProperties = { isExpanded = true; isSelected = false }
//let initialValues = { data = Tree.node {name = "root"; unit = ""} defaultProperties PList.empty }
let initialValues = {
    data = 
        Tree.node { printMessage = text "root" } defaultProperties <| PList.ofList [
            Tree.node { printMessage = text "node 0" } defaultProperties <| PList.ofList [
                Leaf ({ printMessage = text "leaf 00" }, { isSelected = false })
                Leaf ({ printMessage = text "leaf 10" }, { isSelected = false })
            ]
            Tree.node { printMessage = text "node 1" } defaultProperties <| PList.ofList [
                Leaf ({ printMessage = text "leaf 01" }, { isSelected = false })
                Leaf ({ printMessage = text "leaf 11" }, { isSelected = false })
                Tree.node { printMessage = text "node 01" } defaultProperties <| PList.ofList [
                    Leaf ({ printMessage = text "leaf 001" }, { isSelected = false })
                ]
            ]
        ]
    selected = PList.empty
    strgDown = false
}

let updateAt (p : list<Index>) (f : Tree -> Tree) (t : Tree) =
    let rec go (p : list<Index>) (t : Tree)  =
        match p with
            | [] -> f t
            | x::rest -> 
                match t with
                    | Leaf (l, p) -> Leaf (l, p)
                    | Node(l,p,xs) -> 
                        match PList.tryGet x xs with
                            | Some c -> Node(l,p, PList.set x (go rest c) xs)
                            | None   -> t
    go (List.rev p) t

let flipSelected path t b = 
    updateAt path (
        function 
            | Leaf (l, p) -> Leaf (l, {p with isSelected = b })
            | Node (l, p, xs) -> Node (l, { p with 
                                                isSelected = b 
                                                isExpanded = p.isExpanded }, xs)
        ) t

let selectUpdate (oldSelected : plist<list<Index>>) (path : plist<list<Index>>) (tree : Tree) =
    let rec changeSelected (p : plist<list<Index>>) ( t : Tree) ( b : bool )  =
        match PList.tryAt 0 p with
        | None -> t
        | Some c -> changeSelected (PList.removeAt 0 p) (flipSelected c t b) b

    let t = changeSelected oldSelected tree false
    changeSelected path t true

let rec update (m:TreeModel) (msg : Message) =
    match msg with
    | ToggleExpaneded path -> 
        { m with 
            data = 
                updateAt path (
                    function | Leaf (l, p) -> Leaf (l, p)
                                | Node (l, p, xs) -> Node (l, {p with 
                                                                isExpanded = not p.isExpanded 
                                                                isSelected = p.isSelected }, xs )
                ) m.data
        }
    | Selected path ->
        match m.strgDown with
        | false ->
            let oldSelected = m.selected
            { m with
                data = selectUpdate oldSelected (PList.ofList [path]) m.data 
                selected = PList.ofList [path]
            }
        | true ->
            { m with 
                selected = PList.append path m.selected
                data = flipSelected path m.data true
            }
    //if someone presses down both Ctrl Keys, and releases than one, it's not detected correctly
    //it could be solved with an additional variable for left or right and then checking if left or right is pressed
    | Keydown Keys.LeftCtrl -> { m with strgDown = true }
    | Keyup Keys.LeftCtrl -> { m with strgDown = false }
    | Keydown Keys.RightCtrl -> { m with strgDown = true }
    | Keyup Keys.RightCtrl -> { m with strgDown = false }
    | _ -> m 

let highlightStyle =
    "background-color:lightgrey; border: 1px solid grey"

let leafViewText (leaf : MLeafValue) (p : MLeafProperties) (path : list<Index> ) =
    let leaftext = 
        let attr = 
            amap {
                let! isSelected = p.isSelected
                match isSelected with
                | true -> yield attribute "style" highlightStyle
                | false -> yield attribute "style" ""
                yield onMouseClick (fun _ -> Selected path)
            }
        alist {
            let! printMessage = leaf.printMessage
            yield Incremental.span (AttributeMap.ofAMap attr) <| alist {
                yield i [ clazz "file icon";  ] []
                yield printMessage
            }
        }
    Incremental.div 
        (AttributeMap.ofList 
            [ clazz "content";
            ]) leaftext

let nodeViewText (node : MNodeValue) (p : MProperties) (path : list<Index> ) =
    let nodetext = 
        let attr = 
            alist {
                let! isSelected = p.isSelected
                match isSelected with
                | true -> yield attribute "style" highlightStyle
                | false -> yield attribute "style" ""
                yield onMouseClick (fun xs -> Selected path)
            } |> AList.toList
        alist {
            let! printMessage = node.printMessage
            yield div [  
                ] [
                yield span attr [
                    yield printMessage
                ]
            ]
        }
    Incremental.div (AttributeMap.ofList [clazz "content"]) nodetext

let rec traverseTree path (model : IMod<MTree>) =
    alist {
        let! model = model
        match model with
        | MLeaf (l, p) -> 
            let! isSelected = p.isSelected
            yield (leafViewText l p path)
        | MNode (v, p, c) -> 
            let! isSelected = p.isSelected
            
            let nodeAttributes = 
                amap {
                    yield onMouseClick (fun _ -> ToggleExpaneded path)
                    let! isExpanded = p.isExpanded
                    match isExpanded with
                    | true -> yield clazz "icon large outline open folder"
                    | false -> yield clazz "icon large outline folder"
                } |> AttributeMap.ofAMap

            let leafAttributes =
                amap{
                    yield clazz "list"
                    let! isExpanded = p.isExpanded
                    match isExpanded with
                    | true -> yield style "visible"
                    | false -> yield style "display:none"
                } |> AttributeMap.ofAMap

            let children = AList.collecti (fun i v -> traverseTree (i::path) v) c

            yield div [ clazz "item"] [
                Incremental.i nodeAttributes AList.empty
                div [ clazz "content" ][
                    div [ clazz "header" ] [(nodeViewText v p path)]
                    Incremental.div leafAttributes children
                ]
            ]
    }
let view (m: MTreeModel) =
    require Html.semui (
        body [ onKeyDown (fun usedKey -> Keydown usedKey); onKeyUp (fun usedKey -> Keyup usedKey) ][
            h1 [][ text "TreeView" ]
            Incremental.div (AttributeMap.ofList [clazz "ui list"]) (traverseTree [] m.data)
        ]
    )

let threads (model : TreeModel) = 
    ThreadPool.empty

let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = initialValues
        update = update 
        view = view
    }