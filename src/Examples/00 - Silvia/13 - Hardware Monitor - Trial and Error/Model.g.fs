namespace Model

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open Model

[<AutoOpen>]
module Mutable =

    
    
    type MEntry(__initial : Model.Entry) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Model.Entry> = Aardvark.Base.Incremental.EqModRef<Model.Entry>(__initial) :> Aardvark.Base.Incremental.IModRef<Model.Entry>
        let _name = ResetMod.Create(__initial.name)
        let _value = MMap.Create(__initial.value, (fun v -> MOption.Create(v)), (fun (m,v) -> MOption.Update(m, v)), (fun v -> v :> IMod<_>))
        let _sensorType = ResetMod.Create(__initial.sensorType)
        
        member x.name = _name :> IMod<_>
        member x.value = _value :> amap<_,_>
        member x.sensorType = _sensorType :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.Entry) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_name,v.name)
                MMap.Update(_value, v.value)
                ResetMod.Update(_sensorType,v.sensorType)
                
        
        static member Create(__initial : Model.Entry) : MEntry = MEntry(__initial)
        static member Update(m : MEntry, v : Model.Entry) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Model.Entry> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Entry =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let name =
                { new Lens<Model.Entry, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.name
                    override x.Set(r,v) = { r with name = v }
                    override x.Update(r,f) = { r with name = f r.name }
                }
            let value =
                { new Lens<Model.Entry, Aardvark.Base.hmap<System.DateTime,Microsoft.FSharp.Core.option<Microsoft.FSharp.Core.float32>>>() with
                    override x.Get(r) = r.value
                    override x.Set(r,v) = { r with value = v }
                    override x.Update(r,f) = { r with value = f r.value }
                }
            let sensorType =
                { new Lens<Model.Entry, OpenHardwareMonitor.Hardware.SensorType>() with
                    override x.Get(r) = r.sensorType
                    override x.Set(r,v) = { r with sensorType = v }
                    override x.Update(r,f) = { r with sensorType = f r.sensorType }
                }
    
    
    type MHWPart(__initial : Model.HWPart) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Model.HWPart> = Aardvark.Base.Incremental.EqModRef<Model.HWPart>(__initial) :> Aardvark.Base.Incremental.IModRef<Model.HWPart>
        let _hardwareName = ResetMod.Create(__initial.hardwareName)
        let _hardwareType = ResetMod.Create(__initial.hardwareType)
        let _sensor = MMap.Create(__initial.sensor, (fun v -> MEntry.Create(v)), (fun (m,v) -> MEntry.Update(m, v)), (fun v -> v))
        
        member x.hardwareName = _hardwareName :> IMod<_>
        member x.hardwareType = _hardwareType :> IMod<_>
        member x.sensor = _sensor :> amap<_,_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.HWPart) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_hardwareName,v.hardwareName)
                ResetMod.Update(_hardwareType,v.hardwareType)
                MMap.Update(_sensor, v.sensor)
                
        
        static member Create(__initial : Model.HWPart) : MHWPart = MHWPart(__initial)
        static member Update(m : MHWPart, v : Model.HWPart) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Model.HWPart> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module HWPart =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let hardwareName =
                { new Lens<Model.HWPart, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.hardwareName
                    override x.Set(r,v) = { r with hardwareName = v }
                    override x.Update(r,f) = { r with hardwareName = f r.hardwareName }
                }
            let hardwareType =
                { new Lens<Model.HWPart, OpenHardwareMonitor.Hardware.HardwareType>() with
                    override x.Get(r) = r.hardwareType
                    override x.Set(r,v) = { r with hardwareType = v }
                    override x.Update(r,f) = { r with hardwareType = f r.hardwareType }
                }
            let sensor =
                { new Lens<Model.HWPart, Aardvark.Base.hmap<Microsoft.FSharp.Core.string,Model.Entry>>() with
                    override x.Get(r) = r.sensor
                    override x.Set(r,v) = { r with sensor = v }
                    override x.Update(r,f) = { r with sensor = f r.sensor }
                }
    
    
    type MModel(__initial : Model.Model) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Model.Model> = Aardvark.Base.Incremental.EqModRef<Model.Model>(__initial) :> Aardvark.Base.Incremental.IModRef<Model.Model>
        let _hw = MMap.Create(__initial.hw, (fun v -> MHWPart.Create(v)), (fun (m,v) -> MHWPart.Update(m, v)), (fun v -> v))
        let _updateInterval = ResetMod.Create(__initial.updateInterval)
        let _updateTime = ResetMod.Create(__initial.updateTime)
        let _viewInterval = ResetMod.Create(__initial.viewInterval)
        let _threadPool = ResetMod.Create(__initial.threadPool)
        let _selected = MList.Create(__initial.selected)
        
        member x.hw = _hw :> amap<_,_>
        member x.updateInterval = _updateInterval :> IMod<_>
        member x.updateTime = _updateTime :> IMod<_>
        member x.viewInterval = _viewInterval :> IMod<_>
        member x.threadPool = _threadPool :> IMod<_>
        member x.selected = _selected :> alist<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.Model) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MMap.Update(_hw, v.hw)
                ResetMod.Update(_updateInterval,v.updateInterval)
                ResetMod.Update(_updateTime,v.updateTime)
                ResetMod.Update(_viewInterval,v.viewInterval)
                ResetMod.Update(_threadPool,v.threadPool)
                MList.Update(_selected, v.selected)
                
        
        static member Create(__initial : Model.Model) : MModel = MModel(__initial)
        static member Update(m : MModel, v : Model.Model) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Model.Model> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Model =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let hw =
                { new Lens<Model.Model, Aardvark.Base.hmap<Microsoft.FSharp.Core.string,Model.HWPart>>() with
                    override x.Get(r) = r.hw
                    override x.Set(r,v) = { r with hw = v }
                    override x.Update(r,f) = { r with hw = f r.hw }
                }
            let updateInterval =
                { new Lens<Model.Model, System.TimeSpan>() with
                    override x.Get(r) = r.updateInterval
                    override x.Set(r,v) = { r with updateInterval = v }
                    override x.Update(r,f) = { r with updateInterval = f r.updateInterval }
                }
            let updateTime =
                { new Lens<Model.Model, System.DateTime>() with
                    override x.Get(r) = r.updateTime
                    override x.Set(r,v) = { r with updateTime = v }
                    override x.Update(r,f) = { r with updateTime = f r.updateTime }
                }
            let viewInterval =
                { new Lens<Model.Model, System.TimeSpan>() with
                    override x.Get(r) = r.viewInterval
                    override x.Set(r,v) = { r with viewInterval = v }
                    override x.Update(r,f) = { r with viewInterval = f r.viewInterval }
                }
            let threadPool =
                { new Lens<Model.Model, Aardvark.Base.Incremental.ThreadPool<Model.Msg>>() with
                    override x.Get(r) = r.threadPool
                    override x.Set(r,v) = { r with threadPool = v }
                    override x.Update(r,f) = { r with threadPool = f r.threadPool }
                }
            let selected =
                { new Lens<Model.Model, Aardvark.Base.plist<Model.identifier>>() with
                    override x.Get(r) = r.selected
                    override x.Set(r,v) = { r with selected = v }
                    override x.Update(r,f) = { r with selected = f r.selected }
                }
