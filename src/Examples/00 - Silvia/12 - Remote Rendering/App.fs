module App

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Aardvark.Application

//open TreeVApp
//open Model
//
//type Msg =
//    | Clicked of string
//    | TreeMsg of TreeVApp.Message<Msg>
//
//let update (m : Model) (msg : Msg) =
//    //printfn "%A" msg
//    match msg with
//    | Clicked str -> printfn "%A" str; m
//    | TreeMsg (TreeVApp.Message.UserMessage (Clicked s)) -> printfn "inner: %s" s; m
//    | TreeMsg msg -> { m with treeModel = TreeVApp.update m.treeModel msg }
//
//open TreeViewModel
//
//let rec createTreeViewTree (a : UserTree) : InnerTree<string> =
//    match a with
//        | UserNode(s,children) -> InnerNode(s, children |> List.map createTreeViewTree)
//        | UserLeaf(s) -> InnerLeaf s
//
//let view (m: MModel) =
//    let innerTree =
//        m.cTree |> Mod.map createTreeViewTree
//         
//    let tv = TreeVApp.view innerTree (fun s path -> button [onClick (fun _ -> Clicked s)] [text s]) m.treeModel
//
//    require Html.semui (
//        body [attribute "style" "margin:10"] [
//            h1 [][text "Generic TreeView"]
//            tv |> UI.map TreeMsg
//        ]
//    )
//
//let threads (model : Model) = 
//    ThreadPool.empty
//
//let initialValues = { cTree = UserNode("Node 0",[UserNode("Node 10", [UserLeaf "Leaf 010";UserLeaf "Leaf 110"]);UserLeaf "Leaf 00";UserLeaf "Leaf 10"]); treeModel = { collapsed = HSet.empty } }

open HardwareMonitorService
open Model
open OpenHardwareMonitor.Hardware

let sleeptimer = 5  // seconds

type Msg =
    | UpdateHM

let update (m : Hardware) (msg : Msg) =
    match msg with
    | UpdateHM -> 
        { m with hw = updateValues() }

let view (m: MHardware) =
    let atr = amap {
        yield attribute "border" "1"
        yield attribute "style" "border-collapse:collapse"
    } 

    let attrtable = [attribute "style" "padding: 2px"]

    //https://github.com/openhardwaremonitor/openhardwaremonitor/blob/master/Hardware/ISensor.cs
    let unit (x: SensorType) =
        match x with
        | SensorType.Voltage -> "V"
        | SensorType.Clock -> "MHz"
        | SensorType.Temperature -> "°C"
        | SensorType.Load -> "%"
        | SensorType.Fan -> "RPM"
        | SensorType.Flow -> "L/h"
        | SensorType.Control -> "%"
        | SensorType.Level -> "%"
        | SensorType.Factor -> ""
        | SensorType.Power -> "W"
        | SensorType.Data -> "GB"
        //| SensorType.SmallData -> "MB" not included in this library
        | a -> sprintf "%A" a

    let entrylist (sensor : alist<Entry> ) = 
        alist {
            for entry in sensor do
                yield tr attrtable [
                    let value = 
                        match entry.value with 
                        | Some x -> sprintf "%.2f" x
                        | None -> "No value"

                    yield td attrtable [ text entry.name ]
                    yield td (List.append attrtable [attribute "align" "right"]) [ text value ]
                    yield td attrtable [ text (unit entry.sensorType) ]
                ]        
        }

    let tableview =
        alist {
            for hwpart in m.hw do

                yield tr attrtable [
                    yield th (List.append attrtable [attribute "colspan" "3"]) [
                        yield Incremental.text (hwpart.hardwareName |> Mod.map string)
                    ]
                ]
                yield! entrylist hwpart.sensor
        }

    require Html.semui (
        body [attribute "style" "margin:10"] [
            h1 [][text "Hardware Monitor"]

            Incremental.table (AttributeMap.ofAMap atr) tableview
        ]
    )

let initialValues = { hw = PList.empty }

let threads (model : Hardware) = 
    let rec timerProc() =
        proclist {
            yield UpdateHM
            let! _ = Proc.Sleep (sleeptimer * 1000)
            yield! timerProc()
        }
    ThreadPool.add "timer" (timerProc ()) ThreadPool.Empty



let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = initialValues
        update = update 
        view = view
    }
