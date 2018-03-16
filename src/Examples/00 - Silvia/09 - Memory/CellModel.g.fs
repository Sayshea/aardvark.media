namespace CellModel

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open CellModel

[<AutoOpen>]
module Mutable =

    
    
    type MCellModel(__initial : CellModel.CellModel) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<CellModel.CellModel> = Aardvark.Base.Incremental.EqModRef<CellModel.CellModel>(__initial) :> Aardvark.Base.Incremental.IModRef<CellModel.CellModel>
        let _cellstate = ResetMod.Create(__initial.cellstate)
        let _cellText = ResetMod.Create(__initial.cellText)
        
        member x.cellstate = _cellstate :> IMod<_>
        member x.cellText = _cellText :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : CellModel.CellModel) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_cellstate,v.cellstate)
                ResetMod.Update(_cellText,v.cellText)
                
        
        static member Create(__initial : CellModel.CellModel) : MCellModel = MCellModel(__initial)
        static member Update(m : MCellModel, v : CellModel.CellModel) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<CellModel.CellModel> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module CellModel =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let cellstate =
                { new Lens<CellModel.CellModel, CellModel.CellState>() with
                    override x.Get(r) = r.cellstate
                    override x.Set(r,v) = { r with cellstate = v }
                    override x.Update(r,f) = { r with cellstate = f r.cellstate }
                }
            let cellText =
                { new Lens<CellModel.CellModel, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.cellText
                    override x.Set(r,v) = { r with cellText = v }
                    override x.Update(r,f) = { r with cellText = f r.cellText }
                }
