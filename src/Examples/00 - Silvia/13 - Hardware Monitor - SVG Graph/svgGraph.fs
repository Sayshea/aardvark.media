module svgGraph

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open svgGraphModel
open System

let inline (==>) a b = Aardvark.UI.Attributes.attribute a b

let line (f:V2d) (t:V2d) attributes = 
    Svg.line <| attributes @ [
        "x1" ==> sprintf "%f" f.X; "y1" ==> sprintf "%f" f.Y; 
        "x2" ==> sprintf "%f" t.X; "y2" ==> sprintf "%f" t.Y;
    ]

let circle (c:V2d) r attributes =
    Svg.circle <| attributes @ [
        "cx" ==> (sprintf "%f" c.X); "cy" ==> (sprintf "%f" c.Y); "r" ==> (sprintf "%f" r)
    ]

let stext (pos:V2d) attributes =
    Svg.text <| attributes @ [
        "x" ==> sprintf "%f" pos.X; "y" ==> sprintf "%f" pos.Y; 
    ]

//calculates the TimeSpan between a date and DateTime.now, returns the value as Seconds
let convertToSeconds (date:DateTime) =
    let ts = date - DateTime.Now
    ts.TotalSeconds

//convert a timespan value to pixel-coordinates
let convertX xScale (timeInt:TimeSpan) value x0 =
    x0 + (xScale * value / timeInt.TotalSeconds)

//convert the y value to pixel-coordinates
let convertY yScale yOffset yInterval value y0 =
    y0 - (yScale * ((value |> float) - yOffset) / yInterval)

let hsv2rgb (h : float) (s : float) (v : float) =
    let s = clamp 0.0 1.0 s
    let v = clamp 0.0 1.0 v
    let h = h % 1.0
    let h = if h < 0.0 then h + 1.0 else h
    let hi = floor ( h * 6.0 ) |> int
    let f = h * 6.0 - float hi
    let p = v * (1.0 - s)
    let q = v * (1.0 - s * f)
    let t = v * (1.0 - s * ( 1.0 - f ))
    match hi with
//        | 1 -> V3d(q,v,p)
//        | 2 -> V3d(p,v,t)
//        | 3 -> V3d(p,q,v)
//        | 4 -> V3d(t,p,v)
//        | 5 -> V3d(v,p,q)
//        | _ -> V3d(v,t,p)
        | 1 -> V3d(q,p,v)
        | 2 -> V3d(p,t,v)
        | 3 -> V3d(p,v,q)
        | 4 -> V3d(t,v,p)
        | 5 -> V3d(v,q,p)
        | _ -> V3d(v,p,t)

//divides the hue from the hsv color model to try to get seperated colors for the data
let color (i : int) = 
    let golden_ratio_conjugate = 0.618033988749895
    let h = float i * golden_ratio_conjugate
    hsv2rgb h 1.0 0.7

//calculates the rgb value and the offsets for a given index
let getStyling i =
    let rgb = (color i)*255.0
    let r = int rgb.X 
    let g = int rgb.Y 
    let b = int rgb.Z 

    let yoff = float i * -15.0
    { color = sprintf "rgb(%d,%d,%d)" r g b; xOff = 0.; yOff = yoff}

//Calculates the Points to build an empty grid for the diagram
let gridPoints (x0 : float) (x1 : float) (y0 : float) (y1 : float) =
    let OO = V2d (x0, y0)
    let OI = V2d (x0, y1)
    let IO = V2d (x1, y0)
    let II = V2d (x1, y1)
    let x14 = x0 + (x1 - x0) * 0.25
    let x24 = x0 + (x1 - x0) * 0.5
    let x34 = x0 + (x1 - x0) * 0.75
    let y14 = y0 + (y1 - y0) * 0.25
    let y24 = y0 + (y1 - y0) * 0.5
    let y34 = y0 + (y1 - y0) * 0.75

    let OI_14_x = V2d(x14, y0)
    let II_14_x = V2d(x14, y1)
    let OI_24_x = V2d(x24, y0)
    let II_24_x = V2d(x24, y1)
    let OI_34_x = V2d(x34, y0)
    let II_34_x = V2d(x34, y1)

    let OI_14_y = V2d(x0, y14)
    let II_14_y = V2d(x1, y14)
    let OI_24_y = V2d(x0, y24)
    let II_24_y = V2d(x1, y24)
    let OI_34_y = V2d(x0, y34)
    let II_34_y = V2d(x1, y34)

    {
        OO = OO; OI = OI; IO = IO; II = II; 
        OI_14_x = OI_14_x; II_14_x = II_14_x; OI_24_x = OI_24_x; II_24_x = II_24_x; OI_34_x = OI_34_x; II_34_x = II_34_x;
        OI_14_y = OI_14_y; II_14_y = II_14_y; OI_24_y = OI_24_y; II_24_y = II_24_y; OI_34_y = OI_34_y; II_34_y = II_34_y
    }

let graph (xSize : int) (ySize : int ) (timeInt : TimeSpan) (dataObject : alist<dataStructure>) =
    //defines the size of the graph
    //below the x-Axis is more space for the legend of the diagram
    let x0 = (xSize |> float) * 0.1
    let x1 = (xSize |> float) * 0.95
    let y0 = (ySize |> float) / 2. * 0.9
    let y1 = (ySize |> float) * 0.025
    let yScale = y0 - y1
    let xScale = x1 - x0

    let gp = gridPoints x0 x1 y0 y1

    //plots the grid of the diagramm, with lines for 25, 50 and 75% of the values
    let grid  =
        alist {
            yield line gp.OO gp.OI [style "stroke: black; fill: black"]
            yield line gp.OO gp.IO [style "stroke: black; fill: black"]
            yield line gp.IO gp.II [style "stroke: black; fill: black"]
            yield line gp.OI gp.II [style "stroke: black; fill: black"]
            yield line gp.OI_14_x gp.II_14_x [style "stroke: black; fill: black; stroke-dasharray:2,2"]
            yield line gp.OI_24_x gp.II_24_x [style "stroke: black; fill: black; stroke-dasharray:2,2"]
            yield line gp.OI_34_x gp.II_34_x [style "stroke: black; fill: black; stroke-dasharray:2,2"]
            yield line gp.OI_14_y gp.II_14_y [style "stroke: black; fill: black; stroke-dasharray:2,2"]
            yield line gp.OI_24_y gp.II_24_y [style "stroke: black; fill: black; stroke-dasharray:2,2"]
            yield line gp.OI_34_y gp.II_34_y [style "stroke: black; fill: black; stroke-dasharray:2,2"]
        }

    //adds an index to every entry to later add offsets for the Labels
    let dataObjectTransform = dataObject |> AList.toList |> List.mapi (fun i v -> (i,v)) |> AList.ofList

    let dataPoints = 
        alist {
            //Label for the x-Axis
            let timeToString (t:TimeSpan) = t.ToString(@"\:mm\:ss")
            let totalHoursToString (t:TimeSpan) = floor(t.TotalHours)
            let printTime (t:TimeSpan) = sprintf "[-%0.0f%s]" (totalHoursToString t) (timeToString t)
            let t_24 = TimeSpan.FromSeconds(timeInt.TotalSeconds * 0.5)
            yield [
                stext (gp.IO - V2d(0,-14)) [style ("fill:black; text-anchor:middle")] (sprintf "[00:00:00]")
                stext (gp.OO - V2d(0,-14)) [style ("fill:black; text-anchor:left")] (printTime timeInt)
                stext (gp.OI_24_x - V2d(0,-14)) [style ("fill:black; text-anchor:middle")] (printTime t_24)
                stext (gp.IO - V2d(0,-30)) [style ("fill:black; text-anchor:middle")] "last updated:"
            ]

            for index,dataSet in dataObjectTransform do
                //this transformation is to break the incremental update for the values
                //for each new datapoint, every datapoint needs to be reevaluated, as the x value has to change
                let data = dataSet.data |> AMap.toASet |> ASet.toAList |> AList.toMod |> Mod.map Seq.toList
                let! data = data
                
                //values, that are outside the timeinterval, shoudn't be viewed
                let newData = 
                    data
                    |> List.filter (fun (t,v) -> t > DateTime.Now - timeInt)

                let styling = getStyling index

                let minY data = 
                    match data.unit with
                    | "%" -> 0.
                    | _ -> (newData |> List.map (fun (t,v) -> v) |> List.min |> float) * 0.95 |> floor
                let maxY data = 
                    match data.unit with
                    | "%" -> 100.
                    | _ -> (newData |> List.map (fun (t,v) -> v) |> List.max |> float) * 1.05 |> ceil

                //prints the last update time below the x Axis
                match index with
                | 0 -> yield [stext (gp.IO - V2d(0,-45)) [style ("fill:black; text-anchor:middle")] (sprintf "%s" (DateTime.Now.ToLongTimeString()))]
                | _ -> yield []
                
                let count = List.length newData
                match count > 0 with
                | true -> 
                    //draw the points for data, that have values
                    yield [
                        for t,v in newData do
                            let x = convertX xScale timeInt (convertToSeconds t) x1
                            let y = convertY yScale (minY dataSet) ((maxY dataSet) - (minY dataSet)) v y0

                            let domN = 
                                match x < x0 with
                                | true -> None
                                | false -> Some(circle (V2d (x,y)) 1. [style ("stroke: " + styling.color + "; fill:black")])
                            yield domN
                        ] |> List.choose (fun i -> i)
                    //Label for the y-Axis and Legend for the Data
                    let legendPos = gp.OO + V2d(150,60) - V2d(styling.xOff, styling.yOff)
                    yield [
                        stext (gp.OO - V2d(5,-6) - V2d(styling.xOff, styling.yOff)) [style ("fill:" + styling.color + "; text-anchor:end")] (sprintf "%0.0f" (minY dataSet))
                        stext (gp.OI - V2d(5,-6) - V2d(styling.xOff, styling.yOff)) [style ("fill:" + styling.color + "; text-anchor:end")] (sprintf "%0.0f" (maxY dataSet))
                        stext (legendPos) [style ("fill:" + styling.color + "; text-anchor:start")] (sprintf "%s [%s]" dataSet.name dataSet.unit )
                    ]
                | false -> yield []
        }
    
    let dataPoints = dataPoints |> AList.toMod |> Mod.map (Seq.toList >> List.concat)

    let attributes = 
        AttributeMap.ofList [
            attribute "width" (string xSize); attribute "height" (string ySize)
        ]

    Incremental.Svg.svg attributes <| 
        alist {
            yield! grid
            let! dataPoints = dataPoints
            for d in dataPoints do yield d
        }    