module TodoApp_V1

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering
open TodoModel_V1

//for normal list it would work like this:
//let rec getId2 (list: list<int*Index>) (item: Index) =
//    match list with
//    | (i,x)::rest -> 
//        if x = item
//        then i
//        else getId2 rest item
//    | [] -> failwith ""
let indexedList list =
    AList.mapi (fun i x -> i,x) list

let getId (list: alist<Index*Index>) (item: Index) =
    let s =
        AList.toMod list
            |> Mod.map (fun l ->
                l |> Seq.choose (fun (li,idx) ->
                        if idx = item then Some li else None
                ) |> Seq.head
            )
    s |> Mod.force
//    match list with
//    | (i,x)::rest -> 
//        if x = item
//        then i
//        else getId rest item
//    | [] -> failwith ""

let getRealId list id= getId (indexedList list) id

let update (model : TodoModel) (msg : TodoMessage) =
    match msg with
    | AddToTodoList s -> { model with todoList = PList.append s model.todoList }
    | DeleteElement id -> { model with todoList = PList.remove id model.todoList }
    | AddToCompletedList (msg, id) -> 
        match msg with 
        | "true" -> { model with completed = PList.append id model.completed }
        | _ -> failwith ""//{ model with completed = PList.remove (getId (AList.mapi (fun i x -> i,x) model.completed) id) model.completed } //Fehler - id ist nicht die id der completed!!!
    | TestMessage s -> { model with testString = s }

//here it gets wired, because I always have to work with ids so I try another Model

let view (model : MTodoModel) =
    let todoListGui = 
        alist {
             for task in model.todoList do
                yield tr[][
                    yield td [][text task]
                ]
             
             let idList = AList.mapi (fun i x -> i,x) model.todoList
            
//             let rec checkIfChecked id =
//                match model.completed with
//                | a :: rest -> if a = id then true else checkIfChecked rest
//                | [] -> false

             for id,task in idList do
                yield tr [] [
                    yield td [] [
                        input[
                            attribute "type" "checkbox"
                            //onChange (fun msg -> AddToCompletedList (msg, id)) 
                            //onChange (fun msg -> TestMessage msg)

                            //works on the checked Attribute
                            "onchange", 
                            AttributeValue.Event { 
                                clientSide = fun send id -> send id ["event.target.checked"]
                                serverSide = fun client src args -> 
                                    match args with
                                        | a :: _ -> Seq.singleton (AddToCompletedList (a, id))
                                        | _ -> Seq.empty
                            }
                        ]
                    ]
                    
                    yield td [] [text task]

                    yield td [] [
                        text "Id: "
                        text (string id)
                    ]

                    yield td [] [button[onClick (fun _ -> DeleteElement id)] [text "Delete"]]
                ]
  
        }

    let inputListGui = 
        alist {
            for input in model.completed do
                yield text (string input)
                yield br []
        }

    let testMessageGui = 
        alist {
            for input in model.testMessage do
                yield text (string input)
                yield br []
        }

    body [] [
        h1 [] [text "Todo List"]

        input [
            "onchange", 
            AttributeValue.Event { 
                clientSide = fun send id -> send id ["event.target.value"] + "; event.target.value = '';"
                serverSide = fun client src args -> 
                    match args with
                        | a :: _ -> Seq.singleton (AddToTodoList (Pickler.json.UnPickleOfString a))
                        | _ -> Seq.empty
            }
        ]

        Incremental.table AttributeMap.empty todoListGui

        Incremental.div AttributeMap.empty inputListGui

        br []

        //only for testing
        Incremental.div AttributeMap.empty testMessageGui
        Incremental.text ( model.testString |> Mod.map string )
    ]

let threads (model : TodoModel) = 
    ThreadPool.empty

let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = 
            { 
               todoList = PList.ofList []
               completed = PList.ofList []
               testString = ""
               testMessage = PList.ofList []
            }
        update = update 
        view = view
    }