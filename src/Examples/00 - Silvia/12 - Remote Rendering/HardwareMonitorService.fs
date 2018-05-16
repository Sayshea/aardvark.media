module HardwareMonitorService

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental

open OpenHardwareMonitor;
open OpenHardwareMonitor.Hardware;

open Model

let thisComputer = new Computer()
thisComputer.CPUEnabled <- true
thisComputer.GPUEnabled <- true
thisComputer.RAMEnabled <- true

thisComputer.MainboardEnabled <- false //not needed
thisComputer.HDDEnabled <- false //not needed
//thisComputer.FanControllerEnabled <- true // this hadn't any impact on the data viewed

thisComputer.Open()

let null2option arg = if obj.ReferenceEquals(arg, null) then None else Some arg

let nullFloat2optFloat (value : System.Nullable<float32> ) = 
    null2option value.Value

let updateModel (hardwareItem : IHardware) =
    hardwareItem.Update()
    for subHardware in hardwareItem.SubHardware do
        subHardware.Update()

    let s = Seq.map (fun (x:ISensor) -> {name = x.Name; value = nullFloat2optFloat x.Value; sensorType = x.SensorType}) hardwareItem.Sensors
    let sens = PList.ofSeq s
    { hardwareName = hardwareItem.Name; hardwareType = hardwareItem.HardwareType; sensor = sens}


//outdated
//let updateAndPrint (hardwareItem : IHardware) = 
//    hardwareItem.Update()
//    for subHardware in hardwareItem.SubHardware do
//        subHardware.Update()
//
//    printfn "%A:" hardwareItem.Name
//
//    for sensor in hardwareItem.Sensors do
//        let value = nullFloat2optFloat sensor.Value
//
//        match value with
//        | Some v -> 
//            printfn "%A: %f %A" sensor.Name v sensor.SensorType
//        | None ->
//            printfn "%A: no Value" sensor.Name
//    printfn ""

let updateValues() =
//    for hardwareItem in thisComputer.Hardware do
//        updateAndPrint hardwareItem
//        updateModel hardwareItem
//    printfn ""
    let hw = Seq.map (fun hw -> updateModel hw) thisComputer.Hardware
    PList.ofSeq hw

        
//        match hardwareItem.HardwareType with
//        | HardwareType.CPU -> updateAndPrint hardwareItem
//        | HardwareType.GpuAti -> updateAndPrint hardwareItem
//        | HardwareType.GpuNvidia -> updateAndPrint hardwareItem
//        | HardwareType.RAM -> updateAndPrint hardwareItem
//        | s -> 
//            printfn "%A" s

