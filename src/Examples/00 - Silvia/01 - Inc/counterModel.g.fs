namespace counterModel

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open counterModel

[<AutoOpen>]
module Mutable =

    
    
    type MModel(__initial : counterModel.Model) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<counterModel.Model> = Aardvark.Base.Incremental.EqModRef<counterModel.Model>(__initial) :> Aardvark.Base.Incremental.IModRef<counterModel.Model>
        let _counter = ResetMod.Create(__initial.counter)
        
        member x.counter = _counter :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : counterModel.Model) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_counter,v.counter)
                
        
        static member Create(__initial : counterModel.Model) : MModel = MModel(__initial)
        static member Update(m : MModel, v : counterModel.Model) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<counterModel.Model> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Model =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let counter =
                { new Lens<counterModel.Model, Microsoft.FSharp.Core.int>() with
                    override x.Get(r) = r.counter
                    override x.Set(r,v) = { r with counter = v }
                    override x.Update(r,f) = { r with counter = f r.counter }
                }
