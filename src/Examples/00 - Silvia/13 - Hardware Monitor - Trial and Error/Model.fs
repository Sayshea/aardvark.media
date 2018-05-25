namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open Aardvark.UI

open System
open OpenHardwareMonitor
open OpenHardwareMonitor.Hardware

type UpdateModel = {
    date : DateTime
    HwIdent : string
    SensorIdent : string
    value : option<float32>
}
type Msg =
    | UpdateHM
    | UpdateReceived of list<UpdateModel>
    | ChangeInterval of string
    | ChangeViewTime of string
    | ChangeSelected of string * string * string

[<DomainType>]
type Entry = {
    name : string 
    value : hmap<DateTime, option<float32>>
    sensorType : SensorType
}

[<DomainType>]
type HWPart = {
    hardwareName : string
    hardwareType : HardwareType
    sensor : hmap<string, Entry>
}

type identifier = {
    hwpart: string
    sensor: string
}

[<DomainType>]
type Model = {
    hw : hmap<string, HWPart>
    updateInterval : TimeSpan
    updateTime : DateTime
    viewInterval : TimeSpan
    [<NonSerialized>]
    threadPool : ThreadPool<Msg>
    selected : plist<identifier>
}
