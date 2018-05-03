module App

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Aardvark.Application

open Model

open System

let defaultProperties = { isExpanded = true; isSelected = false }
//let initialValues = { data = Tree.node {name = "root"; unit = ""} defaultProperties PList.empty }
let initialValues = {
    data = 
        Tree.node { name = "root"; unit = "" } defaultProperties <| PList.ofList [
            Tree.node { name = "node 0"; unit = "°C" } defaultProperties <| PList.ofList [
                Leaf ({ name = "leaf 00"; value = 20 }, { isSelected = false })
                Leaf ({ name = "leaf 10"; value = 22 }, { isSelected = false })
            ]
            Tree.node { name = "node 1"; unit = "%" } defaultProperties <| PList.ofList [
                Leaf ({ name = "leaf 01"; value = 84 }, { isSelected = false })
                Leaf ({ name = "leaf 11"; value = 54 }, { isSelected = false })
                Tree.node { name = "node 01"; unit = "%" } defaultProperties <| PList.ofList [
                    Leaf ({ name = "leaf 001"; value = 12 }, { isSelected = false })
                ]
            ]
        ]
    selected = PList.empty
    strgDown = false
    drag = None
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

let getItem (p : list<Index>) (t : Tree) =
    let rec go (p : list<Index>) (t : Tree)  =
        match p with
            | [] -> 
                match t with
                    | Leaf (l, p) -> Leaf (l,p)
                    | Node (l,p,xs) -> Node (l,p,xs)
            | x::rest -> 
                match t with
                    | Leaf (l, p) -> failwith "you shoudn't be able to get to this leaf!"//Leaf (l, p)
                    | Node(l,p,xs) -> 
                        match PList.tryGet x xs with
                            | Some c -> go rest c
                            | None   -> failwith "no item found"
    let i = go (List.rev p) t
    i


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



let deleteItem (t : Tree) (itempath : list<Index>) = 
    let parentPath = List.skip 1 itempath
    let tree = updateAt parentPath (
                function | Leaf _ -> failwith "The parent of a leaf must be a node and not a leaf!"
                            | Node (l, p, xs) -> Node (l,p, (PList.remove itempath.Head xs) )
                ) t
    tree

let rec update (m:TreeModel) (msg : Message) =
    match msg with
    | ToggleExpaneded path -> 
        { m with 
            data = 
                updateAt path (
                    function | Leaf (l, p) -> Leaf (l, p)
                                | Node (l, p, xs) -> 
                                    Node (l, {p with 
                                                isExpanded = not p.isExpanded 
                                                isSelected = p.isSelected }, xs )
                ) m.data
        }
    | AddLeaf path -> 
        let genValue =
            let rnd = Random()
            rnd.Next(100)
        let newLeaf = Leaf ({ name = "Generated Leaf"; value = genValue }, { isSelected = false })
        { m with 
            data = 
                updateAt path (
                    function | Leaf _ -> failwith "You can't add anything to a leaf!"
                                | Node (l, p, xs) -> Node (l, p, PList.append (newLeaf) xs)
                ) m.data
        }
    | AddNode path -> 
        let genUnit =
            let rnd = Random()
            let arr = [|"%"; "°C"; "V"; "MHz"; "GB"|]
            arr.[rnd.Next(arr.Length)]
        let newNode = Tree.node { name = "Generated Node"; unit = genUnit } defaultProperties PList.empty
        { m with 
            data = 
                updateAt path (
                    function | Leaf _ -> failwith "You can't add anything to a leaf!"
                                | Node (l, p, xs) -> Node (l, p, PList.append (newNode) xs)
                ) m.data
        }
    | DeleteItem path -> 
        printfn "deleting: %A" path
        //when an Item is deleted, it should get removed from the selected List too
        let pCount = List.length path
        let rec removeSelected ( selected : plist<list<Index>> ) ( newSelected : plist<list<Index>> ) = 
            match PList.tryAt 0 selected with
                | None -> newSelected
                | Some c -> 
                    //need to remove the first elements, because List.forall2 only work on lists with the same size
                    let last n xs = List.toSeq xs |> Seq.skip (xs.Length - n) |> Seq.toList
                    let pEnd = last pCount c
                    match (List.forall2 (fun elem1 elem2 -> elem1 = elem2) pEnd path) with
                    | true -> removeSelected (PList.removeAt 0 selected) newSelected
                    | false -> removeSelected (PList.removeAt 0 selected) (PList.append c newSelected)
        { m with 
            data = deleteItem m.data path
            selected = removeSelected m.selected PList.empty
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
    | DragStop target ->
        match m.drag with
            | None -> m
            | Some source -> 
                //if the target = source, he shouldn't do anything
                match ( source = target ) with
                | true -> m
                | false -> 
                    let AddItem tree s t =
                        let parentPath p = List.skip 1 p // gives back the path of the parent of an item

                        let item tree = 
                            let t = getItem s tree //that's the item that is going to get moved
                            flipSelected [] t false //after the move, the element shouldn't be selected anymore
            
                        let targetItem = getItem t tree //the element, where the item should be added
            
                        match targetItem : Tree with
                            | Leaf _ -> 
                                //you shouldn't add something to a leaf - so get the parent element and add it there
                                let parent = parentPath t
                                updateAt parent (
                                    function | Leaf _ -> failwith "You can't add anything to a leaf!"
                                                | Node (l, p, xs) -> Node (l, p, PList.append (item tree) xs)
                                    ) tree
                            | Node (l, p, xs) -> 
                                updateAt t (
                                    function | Leaf _ -> failwith "You can't add anything to a leaf!"
                                                | Node (l, p, xs) -> Node (l, p, PList.append (item tree) xs)
                                    ) tree

                    //check for Childrens of the current path (source), that are selected and move the childrens first
                    let rec checkForChilds source selectedList model = 
                        let pCount = List.length source
                        match PList.tryAt 0 selectedList with
                        | None -> model
                        | Some c -> 
                            //need to remove the first elements, because List.forall2 only work on lists with the same size
                            let last n xs = List.toSeq xs |> Seq.skip (xs.Length - n) |> Seq.toList
                            let pEnd = last pCount c
                            match (List.forall2 (fun elem1 elem2 -> elem1 = elem2) pEnd source) with
                            | true -> 
                                let m' = update { model with data = AddItem model.data c target } (DeleteItem c)
                                checkForChilds source (PList.removeAt 0 selectedList) m'
                            | false -> checkForChilds source (PList.removeAt 0 selectedList) model
                
                    //move everything, that is selected
                    let rec moveall model =
                        match PList.tryAt 0 model.selected with
                            | None -> model
                            | Some s -> 
                                // when the target is something that is selected, it should get catched here
                                match ( s = target ) with
                                | true -> 
                                    let m' = 
                                        { m with 
                                            selected = PList.removeAt 0 m.selected 
                                            data = flipSelected s m.data false
                                        }
                                    moveall m'
                                | false -> 
                                    let item = getItem s model.data
                                    match item with
                                    | Leaf (l, p) -> 
                                        //Leafs don't have children, so I don't need to check for childrens
                                        let m = { model with data = AddItem model.data s target }
                                        let m' = update m (DeleteItem s)
                                        moveall m'
                                    | Node (l, p, xs) ->
                                        //for nodes I first check, if one of the childrens is in the selected list
                                        let newmodel = checkForChilds s (PList.removeAt 0 model.selected) model
                                        let m = { newmodel with data = AddItem newmodel.data s target }
                                        let m' = update m (DeleteItem s)
                                        moveall m'

                    let mnew = 
                        match m.selected.Count with
                        | 0 -> 
                            //if nothing is selected, it should simply move the node or leaf
                            let m'' = { m with data = AddItem m.data source target }
                            update m'' (DeleteItem source)
                        | _ -> 
                            //when there is something selected, I need to first check for childrens of the source and move them first
                            checkForChilds source m.selected m
                    //move everything that is selected too
                    let m' = moveall mnew
                    { m' with drag = None }

    | DragStart source -> 
        { m with drag = Some source }
    | _ -> m 

let highlightStyle =
    "background-color:lightgrey; border: 1px solid grey"

let dragStart event = 
    "ondragstart", AttributeValue.Event
        { 
            clientSide = fun send id -> 
                String.concat ";" [
                    sprintf "aardvark.processEvent('%s', 'ondragstart');" id
                    "event.dataTransfer.setData('text/plain', 'test');"
                ]
            serverSide = fun _ _ _ -> 
                Seq.delay (event >> Seq.singleton)
        }

let dragStop event = 
    onEvent "ondrop" [] (ignore >> event)

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
            yield Incremental.span (AttributeMap.ofAMap attr) <| alist {
                yield i [ clazz "file icon";  ] []
                yield Incremental.text (leaf.name)
                yield text " = "
                yield Incremental.text (leaf.value |> Mod.map string)
                yield text " "
            }
            yield button [ onClick (fun _ -> DeleteItem path) ][text "x"]
        }
    Incremental.div 
        (AttributeMap.ofList 
            [ clazz "content"; 
              attribute "draggable" "true"; 
              dragStart (fun _ -> DragStart path)
              dragStop (fun _ -> DragStop path); 
              attribute "ondragover" "allowDrop(event)"  
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
            yield div [ 
                attribute "draggable" "true"; 
                dragStart (fun _ -> DragStart path)
                dragStop (fun _ -> DragStop path); 
                attribute "ondragover" "allowDrop(event)" 
                ] [
                yield span attr [
                    yield Incremental.text (node.name)
                    yield text " [Unit: "
                    yield Incremental.text (node.unit)
                    yield text "] "
                ]
                yield button [ onClick (fun _ -> AddNode path) ][text "AddNode"]
                yield button [ onClick (fun _ -> AddLeaf path) ][text "AddLeaf"]
                yield button [ onClick (fun _ -> DeleteItem path) ][text "x"]
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

            match path with
            //the root Element shouldn't be visible, it's only here, to have several nodes at the top level
            | [] -> 
                yield div [ clazz "item"] [
                    button [ onClick (fun _ -> AddNode path) ][text "AddNode"]
                    Incremental.div leafAttributes children
                ]
            | _ -> 
                yield div [ clazz "item"] [
                    Incremental.i nodeAttributes AList.empty
                    div [ clazz "content" ][
                        div [ clazz "header" ] [(nodeViewText v p path)]
                        Incremental.div leafAttributes children
                    ]
                ]
    }
let view (m: MTreeModel) =
    let dependencies = 
        [ 
            { kind = Script; name = "dragDrop"; url = "dragDropTree.js" }
        ]    

    require dependencies (
        require Html.semui (
            body [ attribute "style" "margin:10"; onKeyDown (fun usedKey -> Keydown usedKey); onKeyUp (fun usedKey -> Keyup usedKey) ][
                h1 [][ text "TreeView" ]
                Incremental.div (AttributeMap.ofList [clazz "ui list"]) (traverseTree [] m.data)
            ]
        )
    )
//
//module TreeViewApp =
//    type Model<'a> = {
//        expandedNodes : hset<'a>
//        selected : Option<'a>
//    }
//
//    let view (m : 'a) (children : 'a -> list<'a>) (createUI : 'a -> DomNode<'msg>) (onClick : 'a -> TreeMsg<'msg>) : DomNode<TreeMsg<'msg>> = failwith ""
//
//
//type ComopsedApp = { treeViewModel : TreeViewApp.Model }
//
//let outerViewFunction (m : ComopsedApp) =
//    TreeViewApp.view m.treeViewModel ....

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