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
        let _cells = ResetMod.Create(__initial.cells)
        let _currentPlayer = ResetMod.Create(__initial.currentPlayer)
        
        member x.cells = _cells :> IMod<_>
        member x.currentPlayer = _currentPlayer :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.Model) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_cells,v.cells)
                ResetMod.Update(_currentPlayer,v.currentPlayer)
                
        
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
            let cells =
                { new Lens<Model.Model, Microsoft.FSharp.Core.obj>() with
                    override x.Get(r) = r.cells
                    override x.Set(r,v) = { r with cells = v }
                    override x.Update(r,f) = { r with cells = f r.cells }
                }
            let currentPlayer =
                { new Lens<Model.Model, Microsoft.FSharp.Core.obj>() with
                    override x.Get(r) = r.currentPlayer
                    override x.Set(r,v) = { r with currentPlayer = v }
                    override x.Update(r,f) = { r with currentPlayer = f r.currentPlayer }
                }
