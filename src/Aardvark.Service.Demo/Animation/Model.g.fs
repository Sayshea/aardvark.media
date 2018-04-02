namespace Aardvark.AnimationModel

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.AnimationModel

[<AutoOpen>]
module Mutable =

    
    
    type MTaskProgress(__initial : Aardvark.AnimationModel.TaskProgress) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Aardvark.AnimationModel.TaskProgress> = Aardvark.Base.Incremental.EqModRef<Aardvark.AnimationModel.TaskProgress>(__initial) :> Aardvark.Base.Incremental.IModRef<Aardvark.AnimationModel.TaskProgress>
        let _percentage = ResetMod.Create(__initial.percentage)
        
        member x.percentage = _percentage :> IMod<_>
        member x.startTime = __current.Value.startTime
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Aardvark.AnimationModel.TaskProgress) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_percentage,v.percentage)
                
        
        static member Create(__initial : Aardvark.AnimationModel.TaskProgress) : MTaskProgress = MTaskProgress(__initial)
        static member Update(m : MTaskProgress, v : Aardvark.AnimationModel.TaskProgress) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Aardvark.AnimationModel.TaskProgress> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module TaskProgress =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let percentage =
                { new Lens<Aardvark.AnimationModel.TaskProgress, Microsoft.FSharp.Core.float>() with
                    override x.Get(r) = r.percentage
                    override x.Set(r,v) = { r with percentage = v }
                    override x.Update(r,f) = { r with percentage = f r.percentage }
                }
            let startTime =
                { new Lens<Aardvark.AnimationModel.TaskProgress, System.DateTime>() with
                    override x.Get(r) = r.startTime
                    override x.Set(r,v) = { r with startTime = v }
                    override x.Update(r,f) = { r with startTime = f r.startTime }
                }
    
    
    type MAnimationModel(__initial : Aardvark.AnimationModel.AnimationModel) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Aardvark.AnimationModel.AnimationModel> = Aardvark.Base.Incremental.EqModRef<Aardvark.AnimationModel.AnimationModel>(__initial) :> Aardvark.Base.Incremental.IModRef<Aardvark.AnimationModel.AnimationModel>
        let _cam = ResetMod.Create(__initial.cam)
        let _animation = ResetMod.Create(__initial.animation)
        let _animations = MList.Create(__initial.animations)
        
        member x.cam = _cam :> IMod<_>
        member x.animation = _animation :> IMod<_>
        member x.animations = _animations :> alist<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Aardvark.AnimationModel.AnimationModel) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_cam,v.cam)
                ResetMod.Update(_animation,v.animation)
                MList.Update(_animations, v.animations)
                
        
        static member Create(__initial : Aardvark.AnimationModel.AnimationModel) : MAnimationModel = MAnimationModel(__initial)
        static member Update(m : MAnimationModel, v : Aardvark.AnimationModel.AnimationModel) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Aardvark.AnimationModel.AnimationModel> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module AnimationModel =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let cam =
                { new Lens<Aardvark.AnimationModel.AnimationModel, Aardvark.Base.CameraView>() with
                    override x.Get(r) = r.cam
                    override x.Set(r,v) = { r with cam = v }
                    override x.Update(r,f) = { r with cam = f r.cam }
                }
            let animation =
                { new Lens<Aardvark.AnimationModel.AnimationModel, Aardvark.AnimationModel.Animate>() with
                    override x.Get(r) = r.animation
                    override x.Set(r,v) = { r with animation = v }
                    override x.Update(r,f) = { r with animation = f r.animation }
                }
            let animations =
                { new Lens<Aardvark.AnimationModel.AnimationModel, Aardvark.Base.plist<Aardvark.AnimationModel.Animation<Aardvark.AnimationModel.AnimationModel,Aardvark.Base.CameraView,Aardvark.Base.CameraView>>>() with
                    override x.Get(r) = r.animations
                    override x.Set(r,v) = { r with animations = v }
                    override x.Update(r,f) = { r with animations = f r.animations }
                }
    
    
    type MDemoModel(__initial : Aardvark.AnimationModel.DemoModel) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Aardvark.AnimationModel.DemoModel> = Aardvark.Base.Incremental.EqModRef<Aardvark.AnimationModel.DemoModel>(__initial) :> Aardvark.Base.Incremental.IModRef<Aardvark.AnimationModel.DemoModel>
        let _animations = MAnimationModel.Create(__initial.animations)
        let _cameraState = Aardvark.UI.Primitives.Mutable.MCameraControllerState.Create(__initial.cameraState)
        
        member x.animations = _animations
        member x.cameraState = _cameraState
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Aardvark.AnimationModel.DemoModel) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MAnimationModel.Update(_animations, v.animations)
                Aardvark.UI.Primitives.Mutable.MCameraControllerState.Update(_cameraState, v.cameraState)
                
        
        static member Create(__initial : Aardvark.AnimationModel.DemoModel) : MDemoModel = MDemoModel(__initial)
        static member Update(m : MDemoModel, v : Aardvark.AnimationModel.DemoModel) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Aardvark.AnimationModel.DemoModel> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module DemoModel =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let animations =
                { new Lens<Aardvark.AnimationModel.DemoModel, Aardvark.AnimationModel.AnimationModel>() with
                    override x.Get(r) = r.animations
                    override x.Set(r,v) = { r with animations = v }
                    override x.Update(r,f) = { r with animations = f r.animations }
                }
            let cameraState =
                { new Lens<Aardvark.AnimationModel.DemoModel, Aardvark.UI.Primitives.CameraControllerState>() with
                    override x.Get(r) = r.cameraState
                    override x.Set(r,v) = { r with cameraState = v }
                    override x.Update(r,f) = { r with cameraState = f r.cameraState }
                }
