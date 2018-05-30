namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open Aardvark.UI

open System
open OpenHardwareMonitor
open OpenHardwareMonitor.Hardware

//this Type is used for the update from the HardwareMonitorService
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

//Entries for a single sensor
[<DomainType>]
type Entry = {
    name : string 
    value : hmap<DateTime, option<float32>>
    sensorType : SensorType
}

//Entries for the different HW Parts
//a HWPart can be: CPU, GPU, Memory, etc.
[<DomainType>]
type HWPart = {
    hardwareName : string
    hardwareType : HardwareType
    sensor : hmap<string, Entry>
}

//unique identifier (from the OpenHardwareMonitor Lib), that are used as keys for the maps
//hwpart is the key of hw from the Model
//sensor is the key of sensor from HWPart
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
