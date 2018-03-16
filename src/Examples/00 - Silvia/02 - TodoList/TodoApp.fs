module TodoApp

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering
open TodoModel
open System
open Staging

let updateCompleted tasks guid task c =
    let changedT = { name = task.name; createDate = task.createDate; completed = c }
    let f = function | Some o -> { o with completed = c} | None -> failwith ""
    HMap.update guid f tasks

let updateCount tasks =
    let i = tasks |> HMap.filter (fun _ t -> t.completed = false) |> HMap.count
    match i with
    | 1 -> "1 item left"
    | a -> (string a) + " items left"

let update (model : TaskList) (msg : TodoMessage) =
    let realUpdate model msg =
        match msg with
        | AddToTodoList s -> 
            let task = { name = s; createDate = DateTime.Now; completed = false }
            let guid = System.Guid.NewGuid() |> string
            let t = { model with tasks = HMap.add guid task model.tasks }
            t
        | DeleteElement guid -> { model with tasks = HMap.remove guid model.tasks }
        | AddToCompletedList (msg, g, task) -> 
            match msg with 
            | "true" -> { model with tasks = updateCompleted model.tasks g task true }
            | _ -> { model with tasks = updateCompleted model.tasks g task false }
    let m' = realUpdate model msg
    { m' with activeCount = updateCount m'.tasks }

let view (model : MTaskList) =
    let todoListGui =
        //let sortedList = model.tasks |> ASet.sortBy (fun t -> t.createDate)
        let sortedList = model.tasks |> AMap.toASet |> ASet.sortBy (fun (_,t) -> t.createDate)
        alist {
            for (guid,task) in sortedList do
                yield tr [][
                    yield td [][
                        yield inputCheckbox (fun s -> AddToCompletedList (s, guid, task)) task.completed              
                    ]

                    yield td [
                        (match task.completed with
                            | true -> attribute "style" "text-decoration:line-through"
                            | _ -> attribute "style" "text-decoration:none")
                    ][text task.name]

                    yield td [] [button[onClick (fun _ -> DeleteElement guid)] [text "Delete"]]
                ]
        }

    body [] [
        h1 [] [text "Todos"]

        input [
            attribute "placeholder" "What needs to be done?"
            onChangeResetInput(fun s -> AddToTodoList s)
        ]

        Incremental.table AttributeMap.empty todoListGui

        br []

        Incremental.text ( model.activeCount |> Mod.map string )
    ]

let threads (model : TaskList) = 
    ThreadPool.empty

let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = 
            { 
                tasks = HMap.empty
                activeCount = "0 items left"
            }
        update = update 
        view = view
    }