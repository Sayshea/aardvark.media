module TicTacToeCLogic

open Aardvark.Base

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