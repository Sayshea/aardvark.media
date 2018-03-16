module TicTacToeCApp

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Staging

open TicTacToeCModel

// https://rosettacode.org/wiki/Tic-tac-toe#F.23

let emptyBoard : hmap<(int*int), Brick> =
    HMap.ofList [(0,0), Empty;(0,1), Empty;(0,2), Empty;
        (1,0), Empty;(1,1), Empty;(1,2), Empty;
        (2,0), Empty;(2,1), Empty;(2,2), Empty;]

let get (b : hmap<(int* int), Brick>) (x,y) = b.[y, x]
 
let set player (b : hmap<int*int,Brick>) (x,y) : hmap<int*int,Brick> =
    HMap.add (y,x) player b
 
let winningPositions = 
  [for x in [0..2] -> x,x] // first diagonal
  ::[for x in [0..2] -> 2-x,x] // second diagonal
  ::[for y in [0..2] do
     yield! [[for x in [0..2]->(y,x)]; // columns
             [for x in [0..2] -> (x,y)]]] // rows
 
let hasWon player board = 
  List.exists
    (fun ps -> List.forall (fun pos -> get board pos = player) ps)
    winningPositions
 
let freeSpace board =
  [for x in 0..2 do
     for y in 0..2 do
       if get board (x,y) = Empty then yield x,y]
 
let rec evaluate board move =
  let b2 = set Computer board move
  if hasWon Computer b2 then Win
  else
    match freeSpace b2 with
    | [] -> Draw
    | userChoices -> 
       let b3s = List.map (set User b2) userChoices
       if List.exists (hasWon User) b3s then Lose
       elif List.exists (fun b3 -> bestOutcome b3 = Lose) b3s
       then Lose
       elif List.exists (fun b3 -> bestOutcome b3 = Draw) b3s
       then Draw
       else Win
and findBestChoice b =
  match freeSpace b with
  | [] -> ((-1,-1), Draw)
  | choices -> 
    match List.tryFind (fun c -> evaluate b c = Win) choices with
    | Some c -> (c, Win)
    | None -> match List.tryFind (fun c -> evaluate b c = Draw) choices with
              | Some c -> (c, Draw)
              | None -> (List.head choices, Lose)
and bestOutcome b = snd (findBestChoice b)
 
let bestChoice b = fst (findBestChoice b)
 
let computerPlay b = set Computer b (bestChoice b)

let update (m:Model) (msg : Message) =
    let gameStateUpdate p b = 
        if freeSpace b = [] then GDraw
        elif hasWon p b
        then
            if p = User
            then GWin Player1
            else GWin PC
        else GRunning

    let statusMessageUpdate p gameState =
        match gameState with
        | GRunning -> "Game is running."
        | GWin p -> 
            match p with
            | PC -> "Game over. I have won."
            | Player1 -> "Game over. You have won."
        | GDraw -> "Game over. Draw"

    let gameloop m p =
        let prevUser =
            match p with
            | Player1 -> Computer
            | PC -> User
        let updateModel m = { m with gameState = gameStateUpdate prevUser m.board }
        let m' = updateModel m
        {m' with 
            currentPlayer = p
//            gameState = gameStateUpdate prevUser m.board
            statusMessage = statusMessageUpdate prevUser m'.gameState}
    
    let realUpdate model msg =
        match msg with 
        | PlayBrick (x,y) ->
            match m.gameState with
            | GRunning ->
                let updatePlayer m msg = {m with board = set User m.board (x,y)}
                let mnew = updatePlayer m msg
                gameloop mnew PC
            | _ -> m
        | Restart ->
            let updateBoard m msg = {m with board = emptyBoard}
            let mnew = updateBoard m msg
            gameloop mnew Player1
    let m' = realUpdate m msg

    let ComputerMove m msg = {m with board = set Computer m.board (bestChoice m.board)}
    
    match m'.currentPlayer with
        | PC ->
            let mnew = ComputerMove m' msg
            gameloop mnew Player1
        | _ -> m'

let view (m : MModel) =
    let brickText brick  = 
        brick |> Mod.map ( function
            | Empty -> "-"
            | Computer -> "X"
            | User -> "O"
        )
    
    let styleCell =
            attribute "style" "height:75px; width:75px; font-size:30px; text-align:center; vertical-align:middle; line-height: 75px;"

    let viewcell (brick : IMod<Brick>) x y =
        brick |> Mod.map (function
            | Empty -> button [styleCell; onClick(fun _ -> PlayBrick (x, y))] [Incremental.text (brickText brick |> Mod.map string)]
            | _ -> div [styleCell] [Incremental.text (brickText brick |> Mod.map string)]
        )

    let aget (b : amap<int*int,Brick>) (x,y) = AMap.find (y,x) b

    let lineBricks y =
        alist {
            for x in [0..2] do
                yield ((aget m.board (x,y)),x)
        }
            
    let viewRowCells (col:alist<IMod<Brick> * int>) y = 
        alist {
            for a,x in col do
                let v = viewcell a x y
                let v1 = AList.ofModSingle v

                yield Incremental.td AttributeMap.empty (v1)
        }
    
    let tableview =
        alist {
            for y in [0..2] do
                yield Incremental.tr AttributeMap.empty (viewRowCells (lineBricks y) y)
        }

    body [][
        div [][
            h1 [][text "TicTacToe"]
        
            Incremental.text (m.statusMessage |> Mod.map string)

            Incremental.table AttributeMap.empty tableview

            button[onClick (fun _ -> Restart)] [text "Restart"]
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
                board = emptyBoard
                statusMessage = "Game is running. Your turn."
                currentPlayer = Player1
                gameState = GRunning
            }
        update = update 
        view = view
    }