namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

type Box = int
type Sudoku = Box array array

type Message = 
    | Solve
    | Clear
    | SetInput of int * int * string
    | Example
    | Undo
    | Redo
    | CheckSolvable
    | ChangeSize of string

type History = {
    size : int
    sudoku : Sudoku
}

[<DomainType>]
type Model = 
    {
        gridSize : int
        solvedSudoku : Sudoku
        infoText : string
        prevSudoku : plist<History>
        nextSudoku : plist<History>
    }