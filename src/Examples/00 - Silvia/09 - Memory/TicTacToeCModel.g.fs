namespace TicTacToeCModel

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open TicTacToeCModel

[<AutoOpen>]
module Mutable =

    
    
    type MModel(__initial : TicTacToeCModel.Model) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<TicTacToeCModel.Model> = Aardvark.Base.Incremental.EqModRef<TicTacToeCModel.Model>(__initial) :> Aardvark.Base.Incremental.IModRef<TicTacToeCModel.Model>
        let _board = MMap.Create(__initial.board)
        let _statusMessage = ResetMod.Create(__initial.statusMessage)
        let _gameState = ResetMod.Create(__initial.gameState)
        let _currentPlayer = ResetMod.Create(__initial.currentPlayer)
        
        member x.board = _board :> amap<_,_>
        member x.statusMessage = _statusMessage :> IMod<_>
        member x.gameState = _gameState :> IMod<_>
        member x.currentPlayer = _currentPlayer :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : TicTacToeCModel.Model) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MMap.Update(_board, v.board)
                ResetMod.Update(_statusMessage,v.statusMessage)
                ResetMod.Update(_gameState,v.gameState)
                ResetMod.Update(_currentPlayer,v.currentPlayer)
                
        
        static member Create(__initial : TicTacToeCModel.Model) : MModel = MModel(__initial)
        static member Update(m : MModel, v : TicTacToeCModel.Model) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<TicTacToeCModel.Model> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Model =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let board =
                { new Lens<TicTacToeCModel.Model, Aardvark.Base.hmap<(Microsoft.FSharp.Core.int * Microsoft.FSharp.Core.int),TicTacToeCModel.Brick>>() with
                    override x.Get(r) = r.board
                    override x.Set(r,v) = { r with board = v }
                    override x.Update(r,f) = { r with board = f r.board }
                }
            let statusMessage =
                { new Lens<TicTacToeCModel.Model, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.statusMessage
                    override x.Set(r,v) = { r with statusMessage = v }
                    override x.Update(r,f) = { r with statusMessage = f r.statusMessage }
                }
            let gameState =
                { new Lens<TicTacToeCModel.Model, TicTacToeCModel.GameState>() with
                    override x.Get(r) = r.gameState
                    override x.Set(r,v) = { r with gameState = v }
                    override x.Update(r,f) = { r with gameState = f r.gameState }
                }
            let currentPlayer =
                { new Lens<TicTacToeCModel.Model, TicTacToeCModel.Player>() with
                    override x.Get(r) = r.currentPlayer
                    override x.Set(r,v) = { r with currentPlayer = v }
                    override x.Update(r,f) = { r with currentPlayer = f r.currentPlayer }
                }
