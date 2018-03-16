module App

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Model
open Logic
open Example

//should be 9 or 4 for now
//16 is possible too, but you shoudn't use solve on it - as it may take hours
let size = 9
let emptyS s = seq { for i in 0 .. (s - 1) -> seq { for j in 0 .. (s - 1) -> 0 }} |> toSudoku

let update (m:Model) (msg : Message) =
    match msg with 
        | Solve -> 
            let oldSudoku = m.solvedSudoku
            let solved = getFirstSolution m.solvedSudoku
            match solved with
            | None -> { m with infoText = "Sudoku is not solvable" }
            | Some sudoku -> 
                { m with 
                    solvedSudoku = sudoku
                    infoText = " "
                    prevSudoku = PList.append ({size = m.gridSize; sudoku =  oldSudoku}) m.prevSudoku
                }
        | SetInput (r, c, msg ) ->
            let v : Box = 
                match msg with
                | "" -> 0
                | a -> a |> int
            match (m.solvedSudoku.[c].[r] = v) with
                | true -> m //nothing has changed, so nothing needs to be updated
                | false ->
                    let oldSudoku = m.solvedSudoku
                    let s = replaceAtPos m.solvedSudoku c r v
                    { m with 
                        solvedSudoku = s 
                        infoText = " "
                        prevSudoku = PList.append ({size = m.gridSize; sudoku =  oldSudoku}) m.prevSudoku
                    }
        | Clear -> 
            let oldSudoku = m.solvedSudoku
            { m with 
                solvedSudoku = emptyS m.gridSize
                infoText = " "
                prevSudoku = PList.append ({size = m.gridSize; sudoku =  oldSudoku}) m.prevSudoku
            }
        | Example -> 
            let oldSudoku = m.solvedSudoku
            let toSolve = 
                match m.gridSize with
                | 4 -> s1
                | 16 -> s3
                | _ -> s2
            { m with 
                solvedSudoku = toSolve
                infoText = " "
                prevSudoku = PList.append ({size = m.gridSize; sudoku =  oldSudoku}) m.prevSudoku
            }
        | Undo -> 
            let currentSudoku = m.solvedSudoku
            let lastIndex = PList.count m.prevSudoku - 1
            match lastIndex with
            | -1 -> { m with infoText = "no undo possible" }
            | _ -> 
                { m with
                    solvedSudoku = m.prevSudoku.[lastIndex].sudoku
                    gridSize = m.prevSudoku.[lastIndex].size
                    nextSudoku = PList.append ({size = m.gridSize; sudoku = currentSudoku}) m.nextSudoku
                    prevSudoku = PList.removeAt lastIndex m.prevSudoku
                    infoText = "made an undo"
                }
        | Redo ->
            let currentSudoku = m.solvedSudoku
            let lastIndex = PList.count m.nextSudoku - 1
            match lastIndex with
            | -1 -> { m with infoText = "no redo possible" }
            | _ -> 
                { m with
                    solvedSudoku = m.nextSudoku.[lastIndex].sudoku
                    gridSize = m.prevSudoku.[lastIndex].size
                    prevSudoku = PList.append ({size = m.gridSize; sudoku = currentSudoku}) m.prevSudoku
                    nextSudoku = PList.removeAt lastIndex m.nextSudoku
                    infoText = "made an redo"
                }
        | CheckSolvable -> 
            match (solvable m.solvedSudoku) with
            | true -> { m with infoText = "This Sudoku is solvable" }
            | false -> { m with infoText = "This Sudoku is not solvable" }
        | ChangeSize s -> 
            let currentSudoku = m.solvedSudoku
            let currentSize = m.gridSize
            let size = 
                match s with
                | "2" -> 4
                | "4" -> 16
                | _ -> 9
            { m with 
                gridSize = size
                solvedSudoku = emptyS size
                prevSudoku = PList.append ({size = currentSize; sudoku = currentSudoku}) m.prevSudoku 
                infoText = "changed Size" }

let view (m : MModel) =

    let segment s = 
        match s with
        | 4 -> 2
        | 16 -> 4
        | _ -> 3
    
    let grid = m.gridSize |> Mod.map segment
    
    let colorStyle s =
        match s with
        | "1" -> "#a9f1c2"
        | "2" -> "#c9de7d"
        | "3" -> "#e89ca1"
        | "4" -> "#f5b388"
        | "5" -> "#8cc4e4"
        | "6" -> "#87ecd4"
        | "7" -> "#d0eaa0"
        | "8" -> "#d5afe4"
        | "9" -> "#cad1f5"
        | "10" -> "#5e8be4"
        | "11" -> "#ecd264"
        | "12" -> "#1cd0a4"
        | "13" -> "#c3a51a"
        | "14" -> "#94ca7f"
        | "15" -> "#e4d633"
        | "16" -> "#7f9ede"
        | _ -> "#ffffff"

    let inputCell r c s =
        let atts =             
            amap {
                let! segment = grid
                yield onInput (fun v -> SetInput (r, c, v))
                yield attribute "type" "number"
                // min and max help with the input spinner, but not really change, what you can type in these fields, as it only would validate it when you send it as a form
                yield attribute "min" "1"
                yield attribute "max" (string (segment * segment))
                yield attribute "value" s
                yield attribute "style" ("text-align:center; background-color:" + (colorStyle s))
            }
        
        Incremental.input (AttributeMap.ofAMap atts)

    let viewRowCells r = 
        alist {
            let! segment = grid
            for c in [0..((segment * segment) - 1)] do
                for value in (m.solvedSudoku |> AList.ofModSingle) do 
                let stringValue = 
                    match value.[c].[r] with
                    | 0 -> ""
                    | a -> string a
                yield Incremental.td 
                    (AttributeMap.ofList [style "border: solid thin; height: 1.4em; width: 1.4em; text-align: center; padding: 0"])
                    (alist { yield (inputCell r c stringValue) })
        }

    let tableview =
        alist {
            let! segment = grid
            let cols = 
                [
                for j in [0..(segment - 1)] do
                    yield col []
                ]

            for i in [0..(segment - 1)] do
                yield colgroup [style "border: solid medium;"] [yield! cols]
            for m in [0..(segment - 1)] do
                yield tbody [style "border: solid medium;"][
                    for n in [0..(segment - 1)] do
                        yield Incremental.tr AttributeMap.empty (viewRowCells ((m * segment ) + n))
                ]
        }

    let selectOptions =
        let highlightSelected (sel:int) (value:int) (name:string) =
            if sel = (value * value) 
            then option [attribute "value" (string value); attribute "selected" "selected"] [text name]
            else option [attribute "value" (string value)] [text name]

        alist {
            let! selected = m.gridSize
            yield highlightSelected selected 2 "2x2"
            yield highlightSelected selected 3 "3x3"
            yield highlightSelected selected 4 "4x4"
        }

    require Html.semui ( 
        body [][
            div [][
                h1 [][text "Sudoku Solver"]
        
                text "Size of the Sudoku: "

                Incremental.select (AttributeMap.ofList [onChange(fun msg -> ChangeSize msg)]) selectOptions

                br []
                br []

                Incremental.text (m.infoText |> Mod.map string)

                br []
                br []

                Incremental.table (AttributeMap.ofList [style "border-collapse:collapse; border: solid medium"]) tableview
            
                br []
                button[onClick (fun _ -> Undo)] [text "Undo"]
                button[onClick (fun _ -> Redo)] [text "Redo"]
                button[onClick (fun _ -> CheckSolvable)] [text "Check Solvable"]
            
                br []
                br []

                button[onClick (fun _ -> Solve)] [text "Solve"]
                button[onClick (fun _ -> Clear)] [text "Clear"]
                button[onClick (fun _ -> Example)] [text "Example Sudoku"]

            ]
        ]
    )

let threads (model : Model) = 
    ThreadPool.empty

let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = 
            { 
                gridSize = size
                solvedSudoku = emptyS size
                infoText = " "
                prevSudoku = PList.empty
                nextSudoku = PList.empty
            }
        update = update 
        view = view
    }