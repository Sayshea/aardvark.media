module CompositionApp

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Aardvark.Application

open CompositionModel
open App

//let initialValues : TModel<'a> = {
//    tree = HSet.empty
//}

type Message =
    | Nothing

let update (m:TModel<'a>) (msg : Message) =
    match msg with
    | _ -> failwith ""

let view (m: MTModel<IMod<'a>,'a>) =
    failwith ""


let threads (model : TModel<'a>) = 
    ThreadPool.empty

//Stupid little beast: as the TreeView get's called by another app I don't need this and I don't need to bother about
//type problems caused here for the unpersist
//let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
//    {
//        unpersist = Unpersist.instance     
//        threads = threads 
//        initial = initialValues
//        update = update 
//        view = view
//    }

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
