module App

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering
open Model

let update (model : Model) (msg : Message) =
    match msg with
        | Inc -> { model with value = model.value + 1 }
        | NewName -> 
            let newValue = System.Guid.NewGuid() |> string
            { model with names = PList.append newValue model.names }
        | Select s -> 
            { model with selected = Some s }

let view (model : MModel) =
    
    let namesGui =
        alist {
            for name in model.names do
                yield button [onClick (fun a -> Select name)] [text name]
                yield br []
        }

    let namesGui2 = 
        model.names |> AList.collect (fun str -> 
            AList.ofList [
                text str; br []
            ]
        )

    let selected = 
        let s = model.selected
        let text = s |> Mod.map (fun realSelected -> 
            match realSelected with
                | None -> "nothing is selected"
                | Some s -> s
        )
        Incremental.text text
        
    let selected2 = 
        adaptive {
            let! s = model.selected
            match s with
                | None -> return "ntohg"
                | Some s -> return s
        } |> Incremental.text   

    body [] [
        text "Hello World"
        br []
        button [onClick (fun _ -> Inc)] [text "Increment"]
        text "    "
        Incremental.text (model.value |> Mod.map string)
        br []

        br []
        button [onClick (fun _ -> NewName)] [text "New Name"]
        br []

        text "names:"
        br []
        Incremental.div AttributeMap.empty namesGui
        br []

        text "selected: "
        selected
        br[]

        img [
            attribute "src" "https://upload.wikimedia.org/wikipedia/commons/6/67/SanWild17.jpg"; 
            attribute "alt" "aardvark"
            style "width: 100px"
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
               value = 0
               names = PList.ofList ["aardvark";"super"]
               selected = None
            }
        update = update 
        view = view
    }
