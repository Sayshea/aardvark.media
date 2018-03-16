module TicTacToeCApp

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Staging

open TicTacToeCModel
open TicTacToeCLogic

let csSize = 450
let csSizePart = float csSize / 3.

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

    let getField v = 
        let offset = float csSize * 0.01 //Range when he shouldn't react, cause he could have hit a line

        if offset <= v && v <= (csSizePart - offset)
        then Some 0
        elif (csSizePart + offset) <= v && v <= (csSizePart * 2. - offset)
        then Some 1
        elif (csSizePart * 2. + offset) <= v && v <= (float csSize - offset)
        then Some 2
        else None

    let getField (pos:V2d) =
        let x = getField pos.X
        let y = getField pos.Y
        (x,y)
        
    let realUpdate model msg =
        match msg with 
        | Restart ->
            let updateBoard m msg = {m with board = emptyBoard}
            let mnew = updateBoard m msg
            gameloop mnew Player1
        | CheckPoint pos -> 
            let field = getField pos
            match m.gameState with
            | GRunning ->
                match field with
                | (None, _) -> {m with statusMessage = "Please click inside the boxes!"}
                | (_, None) -> {m with statusMessage = "Please click inside the boxes!"}
                | (Some x, Some y) -> 
                    let br = get m.board (x,y)
                    match br with
                    | Empty ->
                        let updatePlayer m msg = {m with board = set User m.board (x,y)}
                        let mnew = updatePlayer m msg
                        gameloop mnew PC
                    | _ ->
                        {m with statusMessage = "Field is already taken!"}
            | _ -> m

    let m' = realUpdate m msg

    let ComputerMove m msg = {m with board = set Computer m.board (bestChoice m.board)}
    
    match m'.currentPlayer with
        | PC ->
            let mnew = ComputerMove m' msg
            gameloop mnew Player1
        | _ -> m'

let inline (==>) a b = Aardvark.UI.Attributes.attribute a b

let onMouseDownRel (cb : V2d -> 'msg) =
    onEvent "onclick" [sprintf " { X: (getCursor(event)).x.toFixed(), Y: (getCursor(event)).y.toFixed()  }"] (List.head >> (fun a -> Pickler.json.UnPickleOfString a) >> cb)

let view (m : MModel) =
    let line (f:V2d) (t:V2d) attributes = 
        Svg.line <| attributes @ [
            "x1" ==> sprintf "%f" f.X; "y1" ==> sprintf "%f" f.Y; 
            "x2" ==> sprintf "%f" t.X; "y2" ==> sprintf "%f" t.Y;
        ]

//    let stext (pos:V2d) attributes =
//        Svg.text <| attributes @ [
//            "x" ==> sprintf "%f" pos.X; "y" ==> sprintf "%f" pos.Y; 
//            //"alignment-baseline" ==> "middle"; "text-anchor" ==> "middle"
//        ]

    let circle (c:V2d) r attributes =
        Svg.circle <| attributes @ [
            "cx" ==> (sprintf "%f" c.X); "cy" ==> (sprintf "%f" c.Y); "r" ==> (sprintf "%f" r)
        ]

    let drawCross (pos:V2d) =
        let SizePart = csSizePart * 0.75
        alist {
            yield line (new V2d (pos.X + SizePart * 0.5, pos.Y - SizePart * 0.5)) (new V2d(pos.X - SizePart * 0.5, pos.Y + SizePart * 0.5)) [style "stroke-width:4; stroke: black;"]
            yield line (new V2d (pos.X - SizePart * 0.5, pos.Y - SizePart * 0.5)) (new V2d(pos.X + SizePart * 0.5, pos.Y + SizePart * 0.5 )) [style "stroke-width:4; stroke: black;"]
        }

    let drawCircle (pos:V2d) = 
        let SizePart = csSizePart * 0.75
        circle pos (SizePart * 0.5) [style "stroke-width:4; stroke: black; fill:white "]

    let drawPoint (pos:V2d) =
        line pos pos [style "stroke-width:1"]

    let aget (b : amap<int*int,Brick>) (x,y) = AMap.find (y,x) b

    let svg =         
        let attributes = 
            AttributeMap.ofList [
                attribute "width" (string csSize); attribute "height" (string csSize)
                onMouseDownRel CheckPoint
                clazz "svgRoot";
                style "border: 1px solid black;"
            ]

        Incremental.Svg.svg attributes <| 
            alist {
                //H-Line
                for a in [0..1] do
                    let p = float (a + 1) / 3. * float csSize
                    yield line (new V2d (0.,p)) (new V2d ((float csSize),p)) [style "stroke: black; stroke-width:3"]

                //V-Line
                for a in [0..1] do
                    let p = float (a + 1) / 3. * float csSize
                    yield line (new V2d (p, 0.)) (new V2d (p, (float csSize))) [style "stroke: black; stroke-width:3"]

                for y in [0..2] do
                    //let styleText = sprintf "font-size: %fpx" (csSizePart*0.5)
                    let ypos = float y * csSizePart + csSizePart * 0.5
                    for x in [0..2] do
                        let xpos = float x * csSizePart + csSizePart * 0.5
                        let! br = aget m.board (x,y)
                        match br with
//                            | User -> yield stext (new V2d (xpos, ypos)) [style styleText] "X"
//                            | Computer -> yield stext (new V2d (xpos, ypos)) [style styleText] "O"
//                            | Empty -> yield stext (new V2d (xpos, ypos)) [style styleText] ""
                        | User -> yield! drawCross (new V2d (xpos, ypos))
                        | Computer -> yield drawCircle (new V2d (xpos, ypos))
                        | Empty -> yield drawPoint (new V2d (xpos, ypos))
            }

    body [][
        div [][
            h1 [][text "TicTacToe"]
        
            Incremental.text (m.statusMessage |> Mod.map string)
            
            br []

            svg
            
            br []
            
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