module App

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Model

let size = 4

let update (m:Model) (msg : Message) =
    match msg with 
        | Flip p -> m

let view (m : MModel) =


    let cell x y = failwith ""

    let rowview  y=
        alist {
            for x in [0..size] do
                yield Incremental.td AttributeMap.empty (cell x y)
        }


    let tableview = 
        alist {
            for y in [0..size] do
                yield Incremental.tr AttributeMap.empty (rowview y)
        }

    require Html.semui ( 
        body [][
            div [][
                h1 [][text "Memory"]

                Incremental.table (AttributeMap.ofList [style "border-collapse:collapse; border: solid medium"]) tableview
            ]
        ]
    )

//let generateBoard : hmap<pos,card> = 
//    let rand = new System.Random()
//
//    let swap (a: _[]) x y =
//        let tmp = a.[x]
//        a.[x] <- a.[y]
//        a.[y] <- tmp
//
//    // shuffle an array (in-place)
//    let shuffle a =
//        Array.iteri (fun i _ -> swap a i (rand.Next(i, Array.length a))) a
//
//    let size = 4
//    let halfInitArray = seq { 1 .. (size * size / 2)} |> Seq.toList
//    let rec doubleArray x = 
//        match x with
//        | [] -> []
//        | x :: xs -> x :: x :: (doubleArray xs)
//
//    let initArray = (doubleArray halfInitArray)|> List.toArray
//    
//    shuffle initArray
//    
//    let board : hmap<pos,card> = hmap.Empty
////    let x = 0
////    let y = 0
////    let board = HMap.add (x,y) {value = initArray.[(y-1) * size + (x-1)]; flipped = false} board
//    
//    for y in [1 .. size] do
//        for x in [1 .. size] do
//            let board = HMap.add (x,y) {value = initArray.[(y-1) * size + (x-1)]; flipped = false} board
//    board       

//type Brick = int
//
//let get (b : hmap<(int* int), Brick>) (x,y) = b.[y, x]
// 
//let set player (b : hmap<int*int,Brick>) (x,y) : hmap<int*int,Brick> =
//    HMap.add (y,x) player b

let threads (model : Model) = 
    ThreadPool.empty

let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = 
            { 
                numberFlipped = 0
                board = hmap.Empty
                firstFlipped = (0,0)
                secondFlipped = (0,0)
            }
        update = update 
        view = view
    }