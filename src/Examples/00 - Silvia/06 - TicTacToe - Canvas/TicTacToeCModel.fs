namespace TicTacToeCModel

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

type Message = 
    | Restart
    | CheckPoint of V2d

type Player =
    | Player1
    //| Player2
    | PC

type Brick =
    | Empty
    | Computer
    | User

type Evaluation =
    | Win
    | Draw
    | Lose

type GameState =
    | GWin of Player
    | GDraw
    | GRunning

[<DomainType>]
type Model = 
    {
        board : hmap<(int*int),Brick>
        //board : hmap<int, hmap<int, Brick>>  // y -> x -> Brick
        statusMessage : string
        gameState : GameState
        currentPlayer : Player
    }

