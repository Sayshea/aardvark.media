namespace TicTacToeModel

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open TicTacToeModel

[<AutoOpen>]
module Mutable =

    
    
    type MDisplayInfo(__initial : TicTacToeModel.DisplayInfo) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<TicTacToeModel.DisplayInfo> = Aardvark.Base.Incremental.EqModRef<TicTacToeModel.DisplayInfo>(__initial) :> Aardvark.Base.Incremental.IModRef<TicTacToeModel.DisplayInfo>
        let _cells = MList.Create(__initial.cells)
        let _gameStateMessage = ResetMod.Create(__initial.gameStateMessage)
        
        member x.cells = _cells :> alist<_>
        member x.gameStateMessage = _gameStateMessage :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : TicTacToeModel.DisplayInfo) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MList.Update(_cells, v.cells)
                ResetMod.Update(_gameStateMessage,v.gameStateMessage)
                
        
        static member Create(__initial : TicTacToeModel.DisplayInfo) : MDisplayInfo = MDisplayInfo(__initial)
        static member Update(m : MDisplayInfo, v : TicTacToeModel.DisplayInfo) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<TicTacToeModel.DisplayInfo> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module DisplayInfo =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let cells =
                { new Lens<TicTacToeModel.DisplayInfo, Aardvark.Base.plist<TicTacToeModel.Cell>>() with
                    override x.Get(r) = r.cells
                    override x.Set(r,v) = { r with cells = v }
                    override x.Update(r,f) = { r with cells = f r.cells }
                }
            let gameStateMessage =
                { new Lens<TicTacToeModel.DisplayInfo, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.gameStateMessage
                    override x.Set(r,v) = { r with gameStateMessage = v }
                    override x.Update(r,f) = { r with gameStateMessage = f r.gameStateMessage }
                }
    
    
    type MDisplay(__initial : TicTacToeModel.Display) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<TicTacToeModel.Display> = Aardvark.Base.Incremental.EqModRef<TicTacToeModel.Display>(__initial) :> Aardvark.Base.Incremental.IModRef<TicTacToeModel.Display>
        let _gameStateMessage = ResetMod.Create(__initial.gameStateMessage)
        
        member x.gameStateMessage = _gameStateMessage :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : TicTacToeModel.Display) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_gameStateMessage,v.gameStateMessage)
                
        
        static member Create(__initial : TicTacToeModel.Display) : MDisplay = MDisplay(__initial)
        static member Update(m : MDisplay, v : TicTacToeModel.Display) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<TicTacToeModel.Display> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Display =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let gameStateMessage =
                { new Lens<TicTacToeModel.Display, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.gameStateMessage
                    override x.Set(r,v) = { r with gameStateMessage = v }
                    override x.Update(r,f) = { r with gameStateMessage = f r.gameStateMessage }
                }
