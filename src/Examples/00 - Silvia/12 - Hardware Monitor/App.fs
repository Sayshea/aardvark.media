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


//This Value is a TimeSpan for Data, that should be keeped, even when the viewInterval is smaller
//so when you want to zoom in for the last Minute, that you don't loose all the data
let minTime = TimeSpan.FromHours(2.) 

let rec updateModel (m:Model) (updateValues:List<UpdateModel>) =
    match updateValues with
    | [] -> m
    | v::xs -> 
        let optHardwarePart = HMap.tryFind v.HwIdent m.hw
        
        //if new Hardware is added, it's not detected while the app is running, but GPU or CPU shoudn't be changed while the Computer is running
        let newModel = 
            match optHardwarePart with
            | Some hardwarePart -> 
                let optEntryUsed = HMap.tryFind v.SensorIdent hardwarePart.sensor
                let updateMdl =
                    match optEntryUsed with
                    | Some entryUsed -> 
                        let addEntry = { entryUsed with value = HMap.add v.date v.value entryUsed.value }
                        
                        //filter the entries, and remove those, who are older than the viewInterval or minTime
                        let newEntry = 
                            let keepTime = 
                                match m.viewInterval > minTime with
                                | true -> m.viewInterval
                                | _ -> minTime
                            let ts = v.date - keepTime
                            let entries = { addEntry with value = HMap.filter ( fun x _ -> x > ts ) addEntry.value}
                            entries

                        let updateHWPart = function | Some s -> newEntry | None -> failwith ""
                        let newHardwarePart = { hardwarePart with sensor = HMap.update v.SensorIdent updateHWPart hardwarePart.sensor }
                        
                        let updateMdl = function | Some s -> newHardwarePart | None -> failwith ""
                        { m with hw = HMap.update v.HwIdent updateMdl m.hw }
                    | None -> m
                updateMdl
            | None -> m
        let timeUpdatedModel = { newModel with updateTime = v.date }

        updateModel timeUpdatedModel xs


let rec timerProc wait =
    proclist {
        yield UpdateHM
        let! _ = Proc.Sleep wait
        yield! timerProc wait
    }
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
        | true,interval ->
            let threads = 
                ThreadPool.Empty
                    |> ThreadPool.add (System.Guid.NewGuid().ToString()) (timerProc interval)
            { m with updateInterval = (TimeSpan.FromMilliseconds(interval|>float)); threadPool = threads }
        | false,i -> 
            m
    |ChangeViewTime t ->
        match System.Int32.TryParse(t) with
        | true, i ->
            printfn "TimeInterval: %d" i
            { m with viewInterval = (TimeSpan.FromMilliseconds(i|>float))}
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

    // value in milliseconds
    let highlightSelect (sel : TimeSpan) (value : int) (name : string) =
        match (sel = TimeSpan.FromMilliseconds(value |> float)) with
        | true -> option [attribute "value" (string value); attribute "selected" "selected"] [text name]
        | false -> option [attribute "value" (string value)] [text name]

    let selectView (sel : (IMod<TimeSpan>)) (timelist : list<int*string>) =
        alist {
            let! selected = sel
            for time,txt in timelist do
                yield highlightSelect selected time txt
        }

    let timeUpdate = 
        [
            (100, "0.1 s"); (500, "0.5 s"); (1000,"1 s"); (2000,"2 s"); (5000,"5 s"); (10000,"10 s"); (60000,"1 min"); (120000,"2 min");
            (300000,"5 min"); (600000,"10 min"); (1800000,"30 min"); (3600000,"1 h"); (7200000,"2 h"); (21600000,"6 h")
        ]

    let timeView = 
        let tv = 
            [
                (1,"1 min"); (5,"5 min"); (10,"10 min"); (20,"20 min"); (30,"30 min"); (45,"45 min");
                (60,"1 h"); (90,"1.5 h"); (120,"2 h"); (180,"3 h"); (360,"6 h"); (720,"12 h"); (1440,"24 h")
            ]
        tv |> List.map (fun (v,t) -> (v * 60 * 1000, t))


    let dependencies = 
        [ 
            { kind = Script; name = "plotly"; url = "https://cdn.plot.ly/plotly-latest.min.js" }
            { kind = Script; name = "graphScript"; url = "graph.js" }
        ]  
//    let dataChannel = model.data.Channel
//    let updateChart =
//        "data.onmessage = function (values) { if(values.length > 0) refresh(values); };"

    let dataChannel = m.data.Channel
    let plotly = 
        printfn "Plotting Type: %A" dataChannel
        //"plotlyPlot(data)"
        "data.onmessage = function (values) { if(values.length > 0) plotlyPlot(values); };"
    
    
    //"Plotly.newPlot('plotlyGraph', data)"

    //"data.onmessage = function (values) { if(values.length > 0) refresh(values); };"

    require dependencies (
        require Html.semui (
            onBoot' ["data", dataChannel ] plotly (
                body [attribute "style" "margin:10"] [
                    h1 [][text "Hardware Monitor"]

                    div [][
                        div [attribute "style" "float:left; margin-right:10"] [
                            text "Updateinterval: "
                            Incremental.select (AttributeMap.ofList [onChange(fun msg -> ChangeInterval msg)]) (selectView m.updateInterval timeUpdate)
                            br []

                            Incremental.table (AttributeMap.ofAMap atr) tableview
                        ] 
                        div [attribute "style" "float:left"][
                            text "Anzeige Interval: "
                            Incremental.select (AttributeMap.ofList [onChange(fun msg -> ChangeViewTime msg)]) (selectView m.viewInterval timeView)
                            br[]

                            div[attribute "id" "plotlyGraph"][
                                text " "
                            ]
                        ]
                    ]
                ]
            )
        )
    )

let trace1 = " { x : [1, 2, 3, 4], y : [10, 15, 13, 17], type = 'scatter' }; "
let trace2 = " { x : [1, 2, 3, 4], y : [11, 8, 18, 14], type = 'scatter' }; "
//let data = [trace1; trace2]
let dataL = [10;15;13;17]

let initialValues (pool : ThreadPool<_>) = 
    { 
        hw = HMap.empty; 
        updateInterval = TimeSpan.FromMilliseconds(1000.); 
        updateTime = DateTime.Now; 
        viewInterval = TimeSpan.FromMinutes(60.); 
        threadPool = pool 
        data = dataL
    }


let app =    
    let pool = ThreadPool.Empty |> ThreadPool.add (Guid.NewGuid().ToString()) (timerProc 1000)
    {
        unpersist = Unpersist.instance     
        threads = (fun m -> m.threadPool) 
        initial = initialValues pool
        update = update 
        view = view
    }
