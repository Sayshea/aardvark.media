module SilviaApp

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering
open SilviaModel

let update (model : SModel) (msg : SMessage) =
    match msg with
    | SetInput s -> { model with inputString = s }
    | IncCounter -> { model with counter = model.counter + 1 }
    | DecCounter -> { model with counter = model.counter - 1 }
    | SetVariableInput (var, s) -> 
        match var with
        | "inputString1" -> { model with inputString1 = s}
        | "inputString2" -> { model with inputString2 = s}
        | "inputString3" -> { model with inputString3 = s}
        | _ -> failwith ""
    | AddToInputList s -> 
        { model with 
            inputStringUnteres = ""
            inputList = PList.append s model.inputList 
        }

let view (model : MSModel) =
    let inputListGui = 
        alist {
            for input in model.inputList do
                yield text input
                yield br []
        }


    body [] [
        div [] [
            h2 [] [text "Counter:"]

            Incremental.text ( model.counter |> Mod.map string )

            br []

            button [onClick (fun _ -> IncCounter)] [text "Count up"]

            br []

            button [onClick (fun _ -> DecCounter)] [text "Count down"]
        ]

        div [] [
            h2 [] [text "Inputfield:"]

            //onChange - changes the input after hitting enter or losing focus
            //onInput - changes the input while you type
            input [
                onChange (fun s -> SetInput s)
                attribute "type" "text"
            ]

            br[]

            text "Current Input-Text: "

            Incremental.text ( model.inputString |> Mod.map string )        
        ]

        div [] [
            h2 [] [text "Several Inputs:"]

            input [
                onChange (fun s -> SetVariableInput ("inputString1", s))
                attribute "type" "text"
            ]

            text "Current Input-Text: "

            Incremental.text ( model.inputString1 |> Mod.map string ) 
        
            br []

            input [
                onChange (fun s -> SetVariableInput ("inputString2", s))
                attribute "type" "text"
            ]

            text "Current Input-Text: "

            Incremental.text ( model.inputString2 |> Mod.map string ) 
        
            br []

            input [
                onChange (fun s -> SetVariableInput ("inputString3", s))
                attribute "type" "text"
            ]

            text "Current Input-Text: "

            Incremental.text ( model.inputString3 |> Mod.map string ) 
        
            br []

        ]


        div [] [
            h2 [] [text "List of Input Texts:"]

            text "Add to Todo-List:"

            //like the implemented onChange, but it clears the output after hitting enter
            input [
                "onchange", 
                AttributeValue.Event { 
                    clientSide = fun send id -> send id ["event.target.value"] + "; event.target.value = '';"
                    serverSide = fun client src args -> 
                        match args with
                            | a :: _ -> Seq.singleton (AddToInputList (Pickler.json.UnPickleOfString a))
                            | _ -> Seq.empty
                }
                //yield onChange (fun s -> AddToInputList s )
                //yield js "onchange" "event.target.value = '';"
            ]

            Incremental.div AttributeMap.empty inputListGui
        ]

    ]

let threads (model : SModel) = 
    ThreadPool.empty

let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = 
            { 
               inputString = "None"
               counter = 42
               inputString1 = ""
               inputString2 = ""
               inputString3 = ""
               inputList = PList.ofList [""]
               inputStringUnteres = ""
            }
        update = update 
        view = view
    }