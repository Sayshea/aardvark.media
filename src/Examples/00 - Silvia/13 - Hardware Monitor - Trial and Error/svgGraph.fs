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
        //"alignment-baseline" ==> "middle"; "text-anchor" ==> "middle"
    ]

let convertToSeconds (date:DateTime) =
    let ts = date - DateTime.Now
    ts.TotalSeconds

let convertX xScale (timeInt:TimeSpan) value x0 =
    x0 + (xScale * value / timeInt.TotalSeconds)

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
        | 1 -> V3d(q,v,p)
        | 2 -> V3d(p,v,t)
        | 3 -> V3d(p,q,v)
        | 4 -> V3d(t,p,v)
        | 5 -> V3d(v,p,q)
        | _ -> V3d(v,t,p)

let color (i : int) = 
    let golden_ratio_conjugate = 0.618033988749895
    //let h = float i * 73.0 / 360.0
    let h = float i * golden_ratio_conjugate
    hsv2rgb h 1.0 1.0

let indexList i =
    let rgb = (color i)*255.0
    let r = int rgb.X 
    let g = int rgb.Y 
    let b = int rgb.Z 

    let yoff = float i * -12.0
    { color = sprintf "rgb(%d,%d,%d)" r g b; xOff = 0.; yOff = yoff}

type calculatedGridPoints = 
    { 
        OO:V2d; OI:V2d; IO:V2d; II:V2d; 
        OI_14_x:V2d; II_14_x:V2d; OI_24_x:V2d; II_24_x:V2d; OI_34_x:V2d; II_34_x:V2d; 
        OI_14_y:V2d; II_14_y:V2d; OI_24_y:V2d; II_24_y:V2d; OI_34_y:V2d; II_34_y:V2d
    }

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
    let x0 = (xSize |> float) * 0.1
    let x1 = (xSize |> float) * 0.95
    let y0 = (ySize |> float) / 2. * 0.9
    let y1 = (ySize |> float) * 0.025
    let yScale = y0 - y1
    let xScale = x1 - x0

    let gp = gridPoints x0 x1 y0 y1

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

    let dataObjectTransform = dataObject |> AList.toList |> List.mapi (fun i v -> (i,v)) |> AList.ofList

    let dataPoints = 
        alist {
            let timeToString (t:TimeSpan) = t.ToString(@"\:mm\:ss")
            let totalHoursToString (t:TimeSpan) = floor(t.TotalHours)
            let printTime (t:TimeSpan) = sprintf "[-%0.0f%s]" (totalHoursToString t) (timeToString t)
            
            yield [stext (gp.IO - V2d(0,-14)) [style ("fill:black; text-anchor:middle")] (sprintf "[00:00:00]")]
            yield [stext (gp.OO - V2d(0,-14)) [style ("fill:black; text-anchor:left")] (printTime timeInt)]
            let t_24 = TimeSpan.FromSeconds(timeInt.TotalSeconds * 0.5)
            yield [stext (gp.OI_24_x - V2d(0,-14)) [style ("fill:black; text-anchor:middle")] (printTime t_24)]

            //Math.Floor(span.TotalHours) + span.ToString("'h 'm'm 's's'")

            for index,dataSet in dataObjectTransform do
                //handle name etc
                let data = dataSet.data |> AMap.toASet |> ASet.toAList |> AList.toMod |> Mod.map Seq.toList
                let! data = data
                
                let newData = 
                    data
                    |> List.filter (fun (t,v) -> t > DateTime.Now - timeInt)

                let styling = indexList index

                let minY = 
                    match dataSet.unit with
                    | "%" -> 0.
                    | _ -> (newData |> List.map (fun (t,v) -> v) |> List.min |> float) * 0.95 |> floor
                let maxY = 
                    match dataSet.unit with
                    | "%" -> 100.
                    | _ -> (newData |> List.map (fun (t,v) -> v) |> List.max |> float) * 1.05 |> ceil

                yield [
                        for t,v in newData do
                            let x = convertX xScale timeInt (convertToSeconds t) x1
                            let y = convertY yScale minY (maxY - minY) v y0

                            let t = 
                                match x < x0 with
                                | true -> None
                                | false -> Some(circle (V2d (x,y)) 1. [style ("stroke: " + styling.color + "; fill:black")])

                            yield t
                    ] |> List.choose (fun i -> i)
                
                yield [stext (gp.OO - V2d(5,-6) - V2d(styling.xOff, styling.yOff)) [style ("fill:" + styling.color + "; text-anchor:end")] (sprintf ("%0.0f") minY)]
                yield [stext (gp.OI - V2d(5,-6) - V2d(styling.xOff, styling.yOff)) [style ("fill:" + styling.color + "; text-anchor:end")] (sprintf ("%0.0f") maxY)]

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