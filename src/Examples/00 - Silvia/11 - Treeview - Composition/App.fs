module App

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Aardvark.Application

open TreeVApp
open Model

type Msg =
    | Clicked of string
    | TreeMsg of TreeVApp.Message<Msg>

let update (m : Model) (msg : Msg) =
    //printfn "%A" msg
    match msg with
    | Clicked str -> printfn "%A" str; m
    | TreeMsg (TreeVApp.Message.UserMessage (Clicked s)) -> printfn "inner: %s" s; m
    | TreeMsg msg -> { m with treeModel = TreeVApp.update m.treeModel msg }

open TreeViewModel

let rec createTreeViewTree (a : UserTree) : InnerTree<string> =
    match a with
        | UserNode(s,children) -> InnerNode(s, children |> List.map createTreeViewTree)
        | UserLeaf(s) -> InnerLeaf s

let view (m: MModel) =
    let innerTree =
        m.cTree |> Mod.map createTreeViewTree
         
    let tv = TreeVApp.view innerTree (fun s path -> button [onClick (fun _ -> Clicked s)] [text s]) m.treeModel

    require Html.semui (
        body [attribute "style" "margin:10"] [
            h1 [][text "Generic TreeView"]
            tv |> UI.map TreeMsg
        ]
    )

let threads (model : Model) = 
    ThreadPool.empty

let initialValues = { cTree = UserNode("Node 0",[UserNode("Node 10", [UserLeaf "Leaf 010";UserLeaf "Leaf 110"]);UserLeaf "Leaf 00";UserLeaf "Leaf 10"]); treeModel = { collapsed = HSet.empty } }

let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = initialValues
        update = update 
        view = view
    }
