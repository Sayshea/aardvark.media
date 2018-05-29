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

open svgGraph
open svgGraphModel

//This Value is a TimeSpan for Data, that should be keeped, even when the viewInterval is smaller
//so when you want to zoom in for the last Minute, that you don't loose all the data
let minTime = TimeSpan.FromHours(12.) 

//function, that updates the model and remove old entries, that are below the keeptime.
//the keeptime is dependend on the viewInterval and the minTime.
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
                            { addEntry with value = HMap.filter ( fun x _ -> x > ts ) addEntry.value}
                        let updateHWPart = function | Some s -> newEntry | None -> failwith ""
                        let newHardwarePart = { hardwarePart with sensor = HMap.update v.SensorIdent updateHWPart hardwarePart.sensor }
                        let updateMdl = function | Some s -> newHardwarePart | None -> failwith ""
                        { m with hw = HMap.update v.HwIdent updateMdl m.hw }
                    | None -> m
                updateMdl
            | None -> m
        let timeUpdatedModel = { newModel with updateTime = v.date }
        updateModel timeUpdatedModel xs

//thread that calls the Update from the HardwareMonitor, depends on the m.updateTime
let rec timerProc wait =
    proclist {
        yield UpdateHM
        let! _ = Proc.Sleep wait
        yield! timerProc wait
    }

//thread that wait for the values from the sensors 
let updateProc () =
    let update =
        async {
            return updateValues()
        }
    proclist {
        let! vals = Proc.Await update
        yield (UpdateReceived vals)
    }

let update (m : Model) (msg : Msg) =
    match msg with
    //Updates the Values, if 
    | UpdateHM -> 
        match m.hw = hmap.Empty with
        //receives all values from the sensors and generate the hardware structure
        | true ->
            let t, hw = initValues()
            { m with 
                hw = hw
                updateTime = t
            }
        //adds a thread for updating the values from the sensors
        | false ->
            let proc = updateProc()
            let pool = m.threadPool |> ThreadPool.add "updating" proc
            { m with threadPool = pool }
    //gets called by the thread, when he receives the values from the sensors, and updates the values in the model
    | UpdateReceived uv ->
        let m' = updateModel m uv
        { m' with threadPool = m'.threadPool |> ThreadPool.remove "updating"}
    //Change the update Interval based on the selection of the dropdown menu
    //removes the old thread and generate a new one
    | ChangeInterval t -> 
        match System.Int32.TryParse(t) with
        | true,interval ->
            let threads = 
                ThreadPool.Empty
                    |> ThreadPool.add (System.Guid.NewGuid().ToString()) (timerProc interval)
            { m with updateInterval = (TimeSpan.FromMilliseconds(interval|>float)); threadPool = threads }
        | false,i -> 
            m
    //Change the viewInterval for the Graph (x-Axis)
    | ChangeViewTime t ->
        match System.Int32.TryParse(t) with
        | true, i ->
            { m with viewInterval = (TimeSpan.FromMilliseconds(i|>float))}
        | false, i ->
            m
    //Adds or removes entries from the selected plist, based on the identifier
    | ChangeSelected (b, h, s) -> 
        match b with
        | "true" ->
            { m with selected = PList.append { hwpart = h; sensor = s} m.selected }
        | _ -> 
            let t = PList.filter (fun i -> not (i.hwpart = h && i.sensor = s) ) m.selected
            { m with selected = t }

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
        //| SensorType.SmallData -> "MB" not included in this Version of the library
        | a -> sprintf "%A" a

    //generate the table lines for the entries of a hardware part
    let entrylist (sensor : amap<string,MEntry> ) hwIdent = 
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

                let check = 
                    m.selected
                    |> AList.toASet 
                    |> ASet.contains { hwpart = hwIdent; sensor = key}
                let! check = check

                yield tr attrtable [
                    yield td attrtable [ inputCheckbox (fun s -> ChangeSelected (s, hwIdent, key) ) check ]
                    yield td attrtable [ Incremental.text (entry.name |> Mod.map string) ]
                    yield td (List.append attrtable [attribute "align" "right"]) [ text value' ]
                    yield td attrtable [ text (unit sensorType) ]
                ]        
        }

    //generates the view for the table
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
                yield! entrylist hwpart.sensor hwIdent
        }

    //check if the value is selected and returns the line entries for the values
    //time values are in milliseconds [ms]
    let highlightSelect (sel : TimeSpan) (value : int) (name : string) =
        match (sel = TimeSpan.FromMilliseconds(value |> float)) with
        | true -> option [attribute "value" (string value); attribute "selected" "selected"] [text name]
        | false -> option [attribute "value" (string value)] [text name]

    //generates the entries for the dropdown menu
    //int in timelist needs to be in milliseconds
    let dropdownList (sel : (IMod<TimeSpan>)) (timelist : list<int*string>) =
        alist {
            let! selected = sel
            for time,txt in timelist do
                yield highlightSelect selected time txt
        }

    //values for the dropdown menu, how often the values should be updated
    let timeUpdate = 
        [
            (100, "0.1 s"); (500, "0.5 s"); (1000,"1 s"); (2000,"2 s"); (5000,"5 s"); (10000,"10 s"); (60000,"1 min"); (120000,"2 min");
            (300000,"5 min"); (600000,"10 min"); (1800000,"30 min"); (3600000,"1 h"); (7200000,"2 h"); (21600000,"6 h")
        ]

    //values for the dropdown menu, for the x Axis of the diagram
    let timeView = 
        let tv = 
            [
                (1,"1 min"); (5,"5 min"); (10,"10 min"); (20,"20 min"); (30,"30 min"); (45,"45 min");
                (60,"1 h"); (90,"1.5 h"); (120,"2 h"); (180,"3 h"); (360,"6 h"); (720,"12 h"); (1440,"24 h")
            ]
        tv |> List.map (fun (v,t) -> (v * 60 * 1000, t))

    //takes the selected values and plot the svg Graph
    let plotSvgGraph =
        let selected = m.selected |> AList.toASet |> ASet.sortBy (fun t -> t.hwpart)
        alist {
            let genList = List.empty
            let! viewInterval = m.viewInterval

            let plotList = 
                alist{
                    for entry in selected do
                        let! hardwarePart = AMap.find entry.hwpart m.hw
                        let! sensor = AMap.find entry.sensor hardwarePart.sensor
                        let! u = sensor.sensorType
                        let! name = sensor.name
                        let dataList = 
                            sensor.value 
                            |> AMap.filter (fun k _ -> k > (DateTime.Now - viewInterval))
                            |> AMap.chooseM (fun _ v -> v)

                        let data = { name = name; unit = (unit u); data = dataList }
                        yield data
                }
            
            let! count = plotList |> ASet.ofAList |> ASet.count
            match count = 0 with
            | true -> yield text ""
            | false -> 
                yield graph 800 750 viewInterval plotList
        }

    require Html.semui (
        body [attribute "style" "margin:10"] [
            h1 [][text "Hardware Monitor"]

            div [][
                div [attribute "style" "float:left; margin-right:10"] [
                    text "Updateinterval: "
                    Incremental.select (AttributeMap.ofList [onChange(fun msg -> ChangeInterval msg)]) (dropdownList m.updateInterval timeUpdate)
                    br []

                    Incremental.table (AttributeMap.ofAMap atr) tableview
                ] 
                div [attribute "style" "float:left"][
                    text "Anzeige Interval: "
                    Incremental.select (AttributeMap.ofList [onChange(fun msg -> ChangeViewTime msg)]) (dropdownList m.viewInterval timeView)
                    br[]

                    Incremental.div AttributeMap.empty plotSvgGraph
                ]
            ]
        ]
    )

let initialValues (pool : ThreadPool<_>) = 
    { 
        hw = HMap.empty; 
        updateInterval = TimeSpan.FromMilliseconds(1000.); 
        updateTime = DateTime.Now; 
        viewInterval = TimeSpan.FromMinutes(60.); 
        threadPool = pool
        selected = PList.empty
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
