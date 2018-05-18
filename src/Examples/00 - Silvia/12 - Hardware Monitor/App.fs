module App

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Aardvark.Application

open System
open HardwareMonitorService
open Model
open OpenHardwareMonitor.Hardware
open Staging

let minTime = TimeSpan.FromMinutes(60.) 

let rec updateModel (m:Model) (updateValues:List<UpdateModel>) =
    match updateValues with
    | [] -> m
    | v::xs -> 
        let optHardwarePart = HMap.tryFind v.HwIdent m.hw
        
        let newModel = 
            match optHardwarePart with
            | Some hardwarePart -> 
                let optEntryUsed = HMap.tryFind v.SensorIdent hardwarePart.sensor
                let updateMdl =
                    match optEntryUsed with
                    | Some entryUsed -> 
                        let addEntry = { entryUsed with value = HMap.add v.date v.value entryUsed.value }
                        
                        //filter the entries, and remove those, who are older than the maxKeepDataInterval
                        let newEntry = 
                            let ts = v.date - m.maxKeepDataInterval
                            let entries = { addEntry with value = HMap.filter ( fun x _ -> x > ts ) addEntry.value}
                            entries

                        let updateHWPart = function | Some s -> newEntry | None -> failwith ""
                        let newHardwarePart = { hardwarePart with sensor = HMap.update v.SensorIdent updateHWPart hardwarePart.sensor }
                        
                        let updateMdl = function | Some s -> newHardwarePart | None -> failwith ""
                        { m with hw = HMap.update v.HwIdent updateMdl m.hw }

                    | None -> failwith ""
                updateMdl
            | None -> failwith ""
        //TODO: statt failwith ev hinzufügen der daten
        let timeUpdatedModel = { newModel with updateTime = v.date }

        updateModel timeUpdatedModel xs

type Msg =
    | UpdateHM
    | ChangeInterval of string
    | ChangeMaxKeepTime of string

let update (m : Model) (msg : Msg) =
    match msg with
    | UpdateHM -> 
        match m.hw = hmap.Empty with
        | true ->
            let t, hw = initValues()
            { m with 
                hw = hw
                updateTime = t
            }
        | false ->
            let uv = updateValues()
            let m' = updateModel m uv
            m'
    | ChangeInterval t -> 
        match System.Int32.TryParse(t) with
        | true,i ->
            { m with updateInterval = i }
        | false,i -> 
            m
    |ChangeMaxKeepTime t ->
        match System.Int32.TryParse(t) with
        | true, i ->
            { m with maxKeepDataInterval = (TimeSpan.FromSeconds(i|>float))}
        | false, i ->
            m

let view (m: MModel) =
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

    let entrylist (sensor : amap<string,MEntry> ) = 
        let sensorv = sensor
        let s = sensor |> AMap.toASet |> ASet.sortBy (fun (k,v) -> k)
        alist {
            for (key,entry) in s do
                let! sensorType = entry.sensorType
                let! updateTime = m.updateTime
                let! v = AMap.find updateTime entry.value
                let! value = v
                let value' = 
                    match value with 
                    | Some x -> sprintf "%.2f" x
                    | None -> "No value"

                yield tr attrtable [
                    yield td attrtable [ Incremental.text (entry.name |> Mod.map string) ]
                    yield td (List.append attrtable [attribute "align" "right"]) [ text value' ]
                    yield td attrtable [ text (unit sensorType) ]
                ]        
        }

    let tableview =
        let hw = m.hw |> AMap.toASet |> ASet.sortBy (fun (k,t) -> k)
        let l = 1
        alist {
            for (hwIdent,hwpart) in hw do

                yield tr attrtable [
                    yield th (List.append attrtable [attribute "colspan" "3"]) [
                        yield Incremental.text (hwpart.hardwareName |> Mod.map string)
                    ]
                ]
                yield! entrylist hwpart.sensor
        }

//    let selectOptions =
//        let highlightSelected (sel:int) (value:int) (name:string) =
//            if sel = (value * value) 
//            then option [attribute "value" (string value); attribute "selected" "selected"] [text name]
//            else option [attribute "value" (string value)] [text name]
//
//        alist {
//            let! selected = m.gridSize
//            yield highlightSelected selected 2 "2x2"
//            yield highlightSelected selected 3 "3x3"
//            yield highlightSelected selected 4 "4x4"
//        }


    let selectKeepTime =
        let hightlightSelected (sel : TimeSpan) (value : int) (name : string) =
            match (sel = TimeSpan.FromSeconds(value |> float)) with
            | true -> option [attribute "value" (string value); attribute "selected" "selected"] [text name]
            | false -> option [attribute "value" (string value)] [text name]

        alist {
            let! selected = m.maxKeepDataInterval
            yield hightlightSelected selected 60 "1 min"
            yield hightlightSelected selected (5*60) "5 min"
            yield hightlightSelected selected (10*60) "10 min"
            yield hightlightSelected selected (20*60) "20 min"
            yield hightlightSelected selected (30*60) "30 min"
            yield hightlightSelected selected (45*60) "45 min"
            yield hightlightSelected selected (60*60) "1 h"
            yield hightlightSelected selected (90*60) "1.5 h"
            yield hightlightSelected selected (120*60) "2 h"
            yield hightlightSelected selected (180*60) "3 h"
            yield hightlightSelected selected (320*60) "6 h"
            yield hightlightSelected selected (640*60) "12 h"
            yield hightlightSelected selected (1280*60) "24 h"
        }

    require Html.semui (
        body [attribute "style" "margin:10"] [
            h1 [][text "Hardware Monitor"]

            text "Updateinterval: "
            input [ attribute "type" "number"; attribute "value" "1000"; onChange(fun s -> ChangeInterval s) ]

            br []

            text "KeepTime: "
            Incremental.select (AttributeMap.ofList [onChange(fun msg -> ChangeMaxKeepTime msg)]) selectKeepTime

            br[]

            Incremental.table (AttributeMap.ofAMap atr) tableview
        ]
    )

let initialValues = { hw = HMap.empty; updateInterval = 1000; updateTime = DateTime.Now; maxKeepDataInterval = TimeSpan.FromMinutes(1.) }

let threads (model : Model) = 
    let rec timerProc() =
        proclist {
            yield UpdateHM
            let! _ = Proc.Sleep model.updateInterval
            yield! timerProc()
        }
    ThreadPool.add "updateHWMonitor" (timerProc ()) ThreadPool.Empty

let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = initialValues
        update = update 
        view = view
    }
