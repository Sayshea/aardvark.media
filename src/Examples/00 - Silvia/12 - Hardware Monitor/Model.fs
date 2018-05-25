namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open Aardvark.UI

open System
open OpenHardwareMonitor
open OpenHardwareMonitor.Hardware

type Msg =
    | UpdateHM
    | ChangeInterval of string
    | ChangeViewTime of string


type PlottyType = {
    x : list<int>
    y : list<int>
    typeP : string
}


[<DomainType>]
type Entry = {
    name : string 
    value : hmap<DateTime, option<float32>>
    sensorType : SensorType
}

type SensorIdent = string

[<DomainType>]
type HWPart = {
    hardwareName : string
    hardwareType : HardwareType
    sensor : hmap<string, Entry>
}

type HwIdent = string

[<DomainType>]
type Model = {
    hw : hmap<string, HWPart>
    updateInterval : TimeSpan // Time in ms
    updateTime : DateTime
    viewInterval : TimeSpan
    [<NonSerialized>]
    threadPool : ThreadPool<Msg>
    data : list<int>
}

type UpdateModel = {
    date : DateTime
    HwIdent : string
    SensorIdent : string
    value : option<float32>
}