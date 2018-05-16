namespace Model

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open Model

[<AutoOpen>]
module Mutable =

    
    
    type MModel(__initial : Model.Model) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Model.Model> = Aardvark.Base.Incremental.EqModRef<Model.Model>(__initial) :> Aardvark.Base.Incremental.IModRef<Model.Model>
        let _temp = ResetMod.Create(__initial.temp)
        
        member x.temp = _temp :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.Model) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_temp,v.temp)
                
        
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
            let temp =
                { new Lens<Model.Model, Microsoft.FSharp.Core.int>() with
                    override x.Get(r) = r.temp
                    override x.Set(r,v) = { r with temp = v }
                    override x.Update(r,f) = { r with temp = f r.temp }
                }
    
    
    type MHWPart(__initial : Model.HWPart) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Model.HWPart> = Aardvark.Base.Incremental.EqModRef<Model.HWPart>(__initial) :> Aardvark.Base.Incremental.IModRef<Model.HWPart>
        let _hardwareName = ResetMod.Create(__initial.hardwareName)
        let _hardwareType = ResetMod.Create(__initial.hardwareType)
        let _sensor = MList.Create(__initial.sensor)
        
        member x.hardwareName = _hardwareName :> IMod<_>
        member x.hardwareType = _hardwareType :> IMod<_>
        member x.sensor = _sensor :> alist<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.HWPart) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_hardwareName,v.hardwareName)
                ResetMod.Update(_hardwareType,v.hardwareType)
                MList.Update(_sensor, v.sensor)
                
        
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
                { new Lens<Model.HWPart, Aardvark.Base.plist<Model.Entry>>() with
                    override x.Get(r) = r.sensor
                    override x.Set(r,v) = { r with sensor = v }
                    override x.Update(r,f) = { r with sensor = f r.sensor }
                }
    
    
    type MHardware(__initial : Model.Hardware) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Model.Hardware> = Aardvark.Base.Incremental.EqModRef<Model.Hardware>(__initial) :> Aardvark.Base.Incremental.IModRef<Model.Hardware>
        let _hw = MList.Create(__initial.hw, (fun v -> MHWPart.Create(v)), (fun (m,v) -> MHWPart.Update(m, v)), (fun v -> v))
        
        member x.hw = _hw :> alist<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.Hardware) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MList.Update(_hw, v.hw)
                
        
        static member Create(__initial : Model.Hardware) : MHardware = MHardware(__initial)
        static member Update(m : MHardware, v : Model.Hardware) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Model.Hardware> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Hardware =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let hw =
                { new Lens<Model.Hardware, Aardvark.Base.plist<Model.HWPart>>() with
                    override x.Get(r) = r.hw
                    override x.Set(r,v) = { r with hw = v }
                    override x.Update(r,f) = { r with hw = f r.hw }
                }
