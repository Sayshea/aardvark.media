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
        let _gridSize = ResetMod.Create(__initial.gridSize)
        let _solvedSudoku = ResetMod.Create(__initial.solvedSudoku)
        let _infoText = ResetMod.Create(__initial.infoText)
        let _prevSudoku = MList.Create(__initial.prevSudoku)
        let _nextSudoku = MList.Create(__initial.nextSudoku)
        
        member x.gridSize = _gridSize :> IMod<_>
        member x.solvedSudoku = _solvedSudoku :> IMod<_>
        member x.infoText = _infoText :> IMod<_>
        member x.prevSudoku = _prevSudoku :> alist<_>
        member x.nextSudoku = _nextSudoku :> alist<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.Model) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_gridSize,v.gridSize)
                ResetMod.Update(_solvedSudoku,v.solvedSudoku)
                ResetMod.Update(_infoText,v.infoText)
                MList.Update(_prevSudoku, v.prevSudoku)
                MList.Update(_nextSudoku, v.nextSudoku)
                
        
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
            let gridSize =
                { new Lens<Model.Model, Microsoft.FSharp.Core.int>() with
                    override x.Get(r) = r.gridSize
                    override x.Set(r,v) = { r with gridSize = v }
                    override x.Update(r,f) = { r with gridSize = f r.gridSize }
                }
            let solvedSudoku =
                { new Lens<Model.Model, Model.Sudoku>() with
                    override x.Get(r) = r.solvedSudoku
                    override x.Set(r,v) = { r with solvedSudoku = v }
                    override x.Update(r,f) = { r with solvedSudoku = f r.solvedSudoku }
                }
            let infoText =
                { new Lens<Model.Model, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.infoText
                    override x.Set(r,v) = { r with infoText = v }
                    override x.Update(r,f) = { r with infoText = f r.infoText }
                }
            let prevSudoku =
                { new Lens<Model.Model, Aardvark.Base.plist<Model.History>>() with
                    override x.Get(r) = r.prevSudoku
                    override x.Set(r,v) = { r with prevSudoku = v }
                    override x.Update(r,f) = { r with prevSudoku = f r.prevSudoku }
                }
            let nextSudoku =
                { new Lens<Model.Model, Aardvark.Base.plist<Model.History>>() with
                    override x.Get(r) = r.nextSudoku
                    override x.Set(r,v) = { r with nextSudoku = v }
                    override x.Update(r,f) = { r with nextSudoku = f r.nextSudoku }
                }
