module App

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Aardvark.UI.Static

open Model

open System

let inline (==>) a b = Aardvark.UI.Attributes.attribute a b

let setTime () =
    let h = float DateTime.Now.Hour
    let m = float DateTime.Now.Minute
    let s = float DateTime.Now.Second
    { Hours = h + m / 60.0 + s / 3600.0; Minutes = m + s / 60.0; Seconds = s }

let update (m : Model) (msg : Message) =
    match msg with
    | Tick -> 
        //printfn "tick"; 
        { m with currentTime = setTime() }

let view (m : MModel) =
    let csSize = 500
    let svgSize = float csSize * 0.99

    let center : V2d = new V2d ((float csSize/2.0),(float csSize/2.0))
    let length =  (float svgSize/2.0) * 0.9
    let clockSize = (float (svgSize - ((float svgSize) * 0.01))/2.0)

    let calcTimeToCoord hand l : V2d = 
        let clock =
            match hand with
            | Hour n -> n / 12.0
            | Second n -> n / 60.0
            | Minute n -> n / 60.0
        let angle = 2.0 * Math.PI * clock
        let x = center.X + length * l * cos (angle - Math.PI / 2.0)
        let y = center.Y + length * l * sin (angle - Math.PI / 2.0)
        new V2d (x, y)

    //angle in grad
    let polarToKarthesisch angle l : V2d = 
        let angle = angle * Math.PI / 180.
        let x = center.X + l * cos (angle - Math.PI / 2.0)
        let y = center.Y + l * sin (angle - Math.PI / 2.0)
        new V2d (x, y)

    let line (f:V2d) (t:V2d) attributes = 
        Svg.line <| attributes @ [
            "x1" ==> sprintf "%f" f.X; "y1" ==> sprintf "%f" f.Y; 
            "x2" ==> sprintf "%f" t.X; "y2" ==> sprintf "%f" t.Y;
        ]

    let circle (c:V2d) r attributes =
        Svg.circle <| attributes @ [
            "cx" ==> (sprintf "%f" c.X); "cy" ==> (sprintf "%f" c.Y); "r" ==> (sprintf "%f" r)
        ]

    let pointer (t:Times) l style =
        line center (calcTimeToCoord t l) style

    let svg = 
        let clockFacePart angle =
            let pos1 = polarToKarthesisch angle clockSize
            let pos2 = polarToKarthesisch angle (clockSize * 0.9)
            line pos1 pos2 [style "stroke:black; fill:black"]
        
        let attributes = 
            AttributeMap.ofList [
                attribute "width" (string csSize); attribute "height" (string csSize)
            ]

        Incremental.Svg.svg attributes <| 
            alist {
                //clock-shape
                yield circle center clockSize [style "stroke: black; fill:#80808069; stroke-width:2"]

                //Clock - Face-Parts
                for a in 0..12 do
                    yield clockFacePart (float a * 30.0)

                let time = AList.ofModSingle m.currentTime 

                //Pointer for h, m, s
                for t in time do
                    yield pointer (Hour t.Hours) 0.6 [style "stroke:#0000CD; stroke-width:3"]
                    yield pointer (Minute t.Minutes) 0.85 [style "stroke:#FF6347; stroke-width:2"]
                    yield pointer (Second t.Seconds) 1.0 [style "stroke:#26794E; stroke-width:1"]
            }

    body [][
        svg

        button [onClick (fun _ -> Tick)] [text "Start"]
    ]


let threads (model : Model) = 
    let rec timerProc() =
        proclist {
            let! _ = Proc.Sleep 1000
            yield Tick
            yield! timerProc()
        }
    ThreadPool.add "timer" (timerProc ()) ThreadPool.Empty


let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = 
            { 
                currentTime = setTime ()
            }
        update = update 
        view = view
    }