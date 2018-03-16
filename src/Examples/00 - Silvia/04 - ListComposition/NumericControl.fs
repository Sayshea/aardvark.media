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
   
let getValue (m : Model) =
    m.value

let update (m : Model) (a : Action) =
    match a with 
        | Increment -> { m with value = m.value + 0.1 }
        | Decrement -> { m with value = m.value - 0.1 }  
        | Set f -> { m with value = f }

let view (m : MModel) =
    tr[] [
        td[] [a [clazz "ui label circular Big"] [Incremental.text (m.value |> Mod.map(fun x -> sprintf "%.2f" x))]]
        td[] [
            button [clazz "ui icon button"; onClick (fun _ -> Increment)] [text "+"]
            button [clazz "ui icon button"; onClick (fun _ -> Decrement)] [text "-"]                                        
        ]
    ]
