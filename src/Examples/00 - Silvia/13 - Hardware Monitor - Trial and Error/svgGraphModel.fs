namespace svgGraphModel

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open Aardvark.UI

open System

type dataStructure = {
    name : IMod<string>
    unit : string
    data : amap<DateTime, float32>
}

type style = {
    color : string
    xOff : float
    yOff : float
}