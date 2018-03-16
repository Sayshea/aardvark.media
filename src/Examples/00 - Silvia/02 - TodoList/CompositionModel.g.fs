namespace CompositionModel

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open CompositionModel

[<AutoOpen>]
module Mutable =

    
    
    type MVectorModel(__initial : CompositionModel.VectorModel) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<CompositionModel.VectorModel> = Aardvark.Base.Incremental.EqModRef<CompositionModel.VectorModel>(__initial) :> Aardvark.Base.Incremental.IModRef<CompositionModel.VectorModel>
        let _x = NumericModel.Mutable.MModel.Create(__initial.x)
        let _y = NumericModel.Mutable.MModel.Create(__initial.y)
        let _z = NumericModel.Mutable.MModel.Create(__initial.z)
        
        member x.x = _x
        member x.y = _y
        member x.z = _z
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : CompositionModel.VectorModel) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                NumericModel.Mutable.MModel.Update(_x, v.x)
                NumericModel.Mutable.MModel.Update(_y, v.y)
                NumericModel.Mutable.MModel.Update(_z, v.z)
                
        
        static member Create(__initial : CompositionModel.VectorModel) : MVectorModel = MVectorModel(__initial)
        static member Update(m : MVectorModel, v : CompositionModel.VectorModel) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<CompositionModel.VectorModel> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module VectorModel =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let x =
                { new Lens<CompositionModel.VectorModel, NumericModel.Model>() with
                    override x.Get(r) = r.x
                    override x.Set(r,v) = { r with x = v }
                    override x.Update(r,f) = { r with x = f r.x }
                }
            let y =
                { new Lens<CompositionModel.VectorModel, NumericModel.Model>() with
                    override x.Get(r) = r.y
                    override x.Set(r,v) = { r with y = v }
                    override x.Update(r,f) = { r with y = f r.y }
                }
            let z =
                { new Lens<CompositionModel.VectorModel, NumericModel.Model>() with
                    override x.Get(r) = r.z
                    override x.Set(r,v) = { r with z = v }
                    override x.Update(r,f) = { r with z = f r.z }
                }
