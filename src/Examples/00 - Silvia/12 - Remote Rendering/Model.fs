namespace Model

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open Aardvark.UI

open TreeViewModel

open OpenHardwareMonitor
open OpenHardwareMonitor.Hardware;

//type UserTree = UserLeaf of string | UserNode of string * list<UserTree>
//
//[<DomainType>]
//type Model = {
//    [<TreatAsValue>] // we want mod<tree> not mod<usertree>
//    cTree : UserTree
//
//    treeModel : TreeViewModel.TModel
//}

[<DomainType>]
type Model = {
    temp : int
}

type Entry = {
    name : string 
    value : option<float32>
    sensorType : SensorType
}

[<DomainType>]
type HWPart = {
    hardwareName : string
    hardwareType : HardwareType //not sure if I will need this
    sensor : plist<Entry>
}

[<DomainType>]
type Hardware = {
    hw : plist<HWPart>
}