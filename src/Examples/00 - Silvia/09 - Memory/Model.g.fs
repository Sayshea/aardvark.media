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
        let _numberFlipped = ResetMod.Create(__initial.numberFlipped)
        let _board = MMap.Create(__initial.board)
        let _firstFlipped = ResetMod.Create(__initial.firstFlipped)
        let _secondFlipped = ResetMod.Create(__initial.secondFlipped)
        
        member x.numberFlipped = _numberFlipped :> IMod<_>
        member x.board = _board :> amap<_,_>
        member x.firstFlipped = _firstFlipped :> IMod<_>
        member x.secondFlipped = _secondFlipped :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.Model) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_numberFlipped,v.numberFlipped)
                MMap.Update(_board, v.board)
                ResetMod.Update(_firstFlipped,v.firstFlipped)
                ResetMod.Update(_secondFlipped,v.secondFlipped)
                
        
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
            let numberFlipped =
                { new Lens<Model.Model, Microsoft.FSharp.Core.int>() with
                    override x.Get(r) = r.numberFlipped
                    override x.Set(r,v) = { r with numberFlipped = v }
                    override x.Update(r,f) = { r with numberFlipped = f r.numberFlipped }
                }
            let board =
                { new Lens<Model.Model, Aardvark.Base.hmap<Model.pos,Model.card>>() with
                    override x.Get(r) = r.board
                    override x.Set(r,v) = { r with board = v }
                    override x.Update(r,f) = { r with board = f r.board }
                }
            let firstFlipped =
                { new Lens<Model.Model, Model.pos>() with
                    override x.Get(r) = r.firstFlipped
                    override x.Set(r,v) = { r with firstFlipped = v }
                    override x.Update(r,f) = { r with firstFlipped = f r.firstFlipped }
                }
            let secondFlipped =
                { new Lens<Model.Model, Model.pos>() with
                    override x.Get(r) = r.secondFlipped
                    override x.Set(r,v) = { r with secondFlipped = v }
                    override x.Update(r,f) = { r with secondFlipped = f r.secondFlipped }
                }
