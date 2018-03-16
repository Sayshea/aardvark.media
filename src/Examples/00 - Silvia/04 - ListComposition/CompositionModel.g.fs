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
        let _vectorList = MList.Create(__initial.vectorList, (fun v -> NumericModel.Mutable.MModel.Create(v)), (fun (m,v) -> NumericModel.Mutable.MModel.Update(m, v)), (fun v -> v))
        let _numDim = ResetMod.Create(__initial.numDim)
        
        member x.vectorList = _vectorList :> alist<_>
        member x.numDim = _numDim :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : CompositionModel.VectorModel) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MList.Update(_vectorList, v.vectorList)
                ResetMod.Update(_numDim,v.numDim)
                
        
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
            let vectorList =
                { new Lens<CompositionModel.VectorModel, Aardvark.Base.plist<NumericModel.Model>>() with
                    override x.Get(r) = r.vectorList
                    override x.Set(r,v) = { r with vectorList = v }
                    override x.Update(r,f) = { r with vectorList = f r.vectorList }
                }
            let numDim =
                { new Lens<CompositionModel.VectorModel, Microsoft.FSharp.Core.int>() with
                    override x.Get(r) = r.numDim
                    override x.Set(r,v) = { r with numDim = v }
                    override x.Update(r,f) = { r with numDim = f r.numDim }
                }
