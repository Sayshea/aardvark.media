module CompositionApp

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open CompositionModel
open NumericControl

open Staging

type Action = 
    | UpdateX of NumericControl.Action
    | UpdateY of NumericControl.Action
    | UpdateZ of NumericControl.Action
    | Normalize
    | Reset

let initialValues = 
    { 
        x={value = 1.0}
        y={value = 1.0}
        z={value = 1.0}
    }

let update (m : VectorModel) (a : Action) =
    match a with
        | UpdateX a -> { m with x = NumericControl.update m.x a }
        | UpdateY a -> { m with y = NumericControl.update m.y a }
        | UpdateZ a -> { m with z = NumericControl.update m.z a }
        | Normalize -> 
            let v = V3d(m.x.value,m.y.value,m.z.value)
            { m with 
                x = NumericControl.update m.x (Set v.Normalized.X); 
                y = NumericControl.update m.y (Set v.Normalized.Y); 
                z = NumericControl.update m.z (Set v.Normalized.Z) }
        | Reset -> initialValues

let view (m : MVectorModel) =
    require Html.semui (             
        div[][
            table [] [
                tr[] [
                    td[][a [clazz "ui label circular Big"][text "X:"]]
                    td[][NumericControl.view m.x |> UI.map UpdateX]
                ]
                tr[] [
                    td[][a [clazz "ui label circular Big"][text "Y:"]]
                    td[][NumericControl.view m.y |> UI.map UpdateY]
                ]
                tr[] [
                    td[][a [clazz "ui label circular Big"][text "Z:"]]
                    td[][NumericControl.view m.z |> UI.map UpdateZ]
                ]
                tr[] [
                    td[attribute "colspan" "2"][
                        div[clazz "ui buttons small"][
                            button [clazz "ui button"; onClick (fun _ -> Normalize)] [text "Norm"]
                            button [clazz "ui button"; onClick (fun _ -> Reset)] [text "Reset"]
                        ]
                    ]
                ]
            ]               
        ]
    )

let view2 (m : MVectorModel) =
    require Html.semui (             
        div[][
            table [] [
                NumericControl.view2 m.x "X:" |> UI.map UpdateX
                NumericControl.view2 m.y "Y:" |> UI.map UpdateY
                NumericControl.view2 m.z "Z:" |> UI.map UpdateZ
                tr[][
                    td[attribute "colspan" "2"][
                        div[clazz "ui buttons small"][
                            button [clazz "ui button"; onClick (fun _ -> Normalize)] [text "Norm"]
                            button [clazz "ui button"; onClick (fun _ -> Reset)] [text "Reset"]
                        ]
                    ]
                ]
            ]               
        ]
    )

let app =
    {
        unpersist = Unpersist.instance        
        threads   = fun _ -> ThreadPool.empty 
        initial   = initialValues
        update    = update
        view      = view2
    }