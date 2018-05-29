module HardwareMonitorService

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental

open OpenHardwareMonitor
open OpenHardwareMonitor.Hardware

open Model
open System

//http://openhardwaremonitor.org/wordpress/wp-content/uploads/2011/04/OpenHardwareMonitor-WMI.pdf

let thisComputer = new Computer()
//sets which values should be 
thisComputer.CPUEnabled <- true
thisComputer.GPUEnabled <- true
thisComputer.RAMEnabled <- true

thisComputer.MainboardEnabled <- false //not needed
thisComputer.HDDEnabled <- false //not needed

thisComputer.Open()

let null2option arg = if obj.ReferenceEquals(arg, null) then None else Some arg

let nullFloat2optFloat (value : System.Nullable<float32> ) = 
    null2option value |> Option.bind (fun value -> null2option value.Value)

//updates the values of a sensor and returns these values
let initModel (hardwareItem : IHardware) datetime =
    hardwareItem.Update()
    for subHardware in hardwareItem.SubHardware do
        subHardware.Update()

    let s = Seq.map (fun (x:ISensor) -> (sprintf "%A" x.Identifier) , {name = x.Name; value = hmap.OfSeq [ datetime, (nullFloat2optFloat x.Value)]; sensorType = x.SensorType}) hardwareItem.Sensors
    let sens = HMap.ofSeq s
    (sprintf "%A" hardwareItem.Identifier), { hardwareName = hardwareItem.Name; hardwareType = hardwareItem.HardwareType; sensor = sens}

//return all values from the sensors for each Hardware Part
//this function is called, only when there was no hardware in the model before
let initValues() =
    let datetime = DateTime.Now
    let hw = Seq.map (fun hw -> initModel hw datetime) thisComputer.Hardware
    datetime,(HMap.ofSeq hw)

//updates the values for each sensor and returns a seq of values to update the model
let updateModel (hardwareItem : IHardware) (datetime : DateTime) : seq<UpdateModel> =
    hardwareItem.Update()
    for subHardware in hardwareItem.SubHardware do
        subHardware.Update()
    Seq.map (fun (x:ISensor) -> { date = datetime; HwIdent = (sprintf "%A" hardwareItem.Identifier); SensorIdent = (sprintf "%A" x.Identifier); value = (nullFloat2optFloat x.Value)}) hardwareItem.Sensors

//this function is called for every update that is called from the thread
//it returns a List of values to update the model
let updateValues() : List<UpdateModel> =
    let datetime = DateTime.Now
    (Seq.map (fun hw -> updateModel hw datetime) thisComputer.Hardware) |> Seq.concat |> Seq.toList
    
