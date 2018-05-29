namespace svgGraphModel

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open Aardvark.UI

open System

type dataStructure = {
    name : string
    unit : string
    data : amap<DateTime, float32>
}

type style = {
    color : string
    xOff : float
    yOff : float
}

type calculatedGridPoints = 
    { 
        OO:V2d; OI:V2d; IO:V2d; II:V2d; 
        OI_14_x:V2d; II_14_x:V2d; OI_24_x:V2d; II_24_x:V2d; OI_34_x:V2d; II_34_x:V2d; 
        OI_14_y:V2d; II_14_y:V2d; OI_24_y:V2d; II_24_y:V2d; OI_34_y:V2d; II_34_y:V2d
    }