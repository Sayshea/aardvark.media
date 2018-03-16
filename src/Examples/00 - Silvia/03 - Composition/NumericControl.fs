module NumericControl

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open NumericModel

type Action = 
    | Increment
    | Decrement
    | Set of float
    
let update (m : Model) (a : Action) =
    match a with 
        | Increment -> { m with value = m.value + 0.1 }
        | Decrement -> { m with value = m.value - 0.1 }  
        | Set f -> { m with value = f }

let view (m : MModel) =
    require Html.semui (         
        table [][ 
            tr[] [
                td[] [a [clazz "ui label circular Big"] [Incremental.text (m.value |> Mod.map(fun x -> " " + x.ToString()))]]
                td[] [
                    button [clazz "ui icon button"; onClick (fun _ -> Increment)] [text "+"]
                    button [clazz "ui icon button"; onClick (fun _ -> Decrement)] [text "-"]                                        
                ]
            ]
        ]        
    )

let view2 (m : MModel) (direction : string) =
    tr[] [
        td[] [a [clazz "ui label circular Big"] [text direction]]
        td[] [a [clazz "ui label circular Big"] [Incremental.text (m.value |> Mod.map(fun x -> " " + x.ToString()))]]
        td[] [
            button [clazz "ui icon button"; onClick (fun _ -> Increment)] [text "+"]
            button [clazz "ui icon button"; onClick (fun _ -> Decrement)] [text "-"]                                        
        ]
    ]

//let app =
//    {
//        unpersist = Unpersist.instance        //ignore for now
//        threads   = fun _ -> ThreadPool.empty //ignore for now
//        initial   = { value = 0.0 }
//        update    = update
//        view      = view'
//    }