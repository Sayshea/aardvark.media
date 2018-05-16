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
    | AddDimension
    | DelDimension
    | Update' of Index * NumericControl.Action
    | Normalize
    | Reset
    | ResetAll

let update (m : VectorModel) (a : Action) =
    match a with
        | AddDimension -> 
            { m with 
                vectorList = PList.append { value = 1.0 } m.vectorList 
                numDim = m.vectorList.Count + 1
            }
        | DelDimension -> 
            let i = m.vectorList.Count - 1
            let j = 
                match i with
                | -1 -> 0
                | a -> a
            { m with 
                vectorList = PList.removeAt i m.vectorList
                numDim = j
            }
        | Update'(i,a) -> 
            let v = 
                let v1 = PList.tryGet i m.vectorList
                match v1 with
                    | Some v' -> v'
                    | None -> failwith ""
            let v1 = NumericControl.update v a
            { m with vectorList = PList.set i v1 m.vectorList }
        | Normalize ->
            let z = PList.toList m.vectorList
            let z'= List.fold (fun norm v -> norm + (NumericControl.getValue v) * (NumericControl.getValue v) ) 0.0  z
            //let z'= List.fold (fun norm v -> norm + v.value * v.value ) 0.0  z
            { m with vectorList = PList.map (fun vector -> NumericControl.update vector (Set ( (NumericControl.getValue vector)/z'))) m.vectorList}
        | Reset -> {m with vectorList = PList.map (fun vector -> NumericControl.update vector (Set 1.0)) m.vectorList }
        | ResetAll -> 
            {m with 
                vectorList = PList.empty 
                numDim = 0
            }

let view (m : MVectorModel) =
    let models = 
        m.vectorList |> AList.mapi (fun i vector ->
            NumericControl.view vector |> UI.map (fun a -> Update'(i,a)) 
        )

    let countedDim = AList.count m.vectorList

    require Html.semui (             
        body[attribute "style" "margin:10"][
            h1 [] [text "n-dim Vectors"]

            text "Add or remove dimensions: "
            button [onClick(fun _ -> AddDimension)] [text "+"]
            button [onClick(fun _ -> DelDimension)] [text "-"]

            Incremental.table AttributeMap.empty models    
            
            button [onClick(fun _ -> Normalize)] [text "Normalize"]
            button [onClick(fun _ -> Reset)] [text "Reset Values"]
            button [onClick(fun _ -> ResetAll)] [text "Delete all Dimensions"]
            br []
            text "Number of dimensions (from model): "
            Incremental.text (m.numDim |> Mod.map (fun x -> sprintf "%i" x))
            br []
            text "Number of dimensions (counted): "
            Incremental.text (countedDim |> Mod.map (fun x -> sprintf "%i" x))
        ]
    )

let app =
    {
        unpersist = Unpersist.instance        
        threads   = fun _ -> ThreadPool.empty 
        initial   = 
            {
                vectorList = PList.empty
                numDim = 0
            }
        update    = update
        view      = view
    }