module counterApp

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open counterModel

let update (model : Model) (msg : Message) =
    match msg with
    | IncCounter -> { model with counter = model.counter + 1 }
    | DecCounter -> { model with counter = model.counter - 1 }

let view (model : MModel) =
    body [] [
        div [] [
            h2 [] [text "Counter:"]
            Incremental.text ( model.counter |> Mod.map string )
            br []
            button [onClick (fun _ -> IncCounter)] [text "Count up"]
            br []
            button [onClick (fun _ -> DecCounter)] [text "Count down"]
        ]
    ]

let threads (model : Model) = 
    ThreadPool.empty

let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = 
            { 
               counter = 42
            }
        update = update 
        view = view
    }