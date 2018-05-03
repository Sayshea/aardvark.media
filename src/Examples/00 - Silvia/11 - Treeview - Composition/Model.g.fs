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
        let _cTree = ResetMod.Create(__initial.cTree)
        let _treeModel = TreeViewModel.Mutable.MTModel.Create(__initial.treeModel)
        
        member x.cTree = _cTree :> IMod<_>
        member x.treeModel = _treeModel
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.Model) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                _cTree.Update(v.cTree)
                TreeViewModel.Mutable.MTModel.Update(_treeModel, v.treeModel)
                
        
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
            let cTree =
                { new Lens<Model.Model, Model.UserTree>() with
                    override x.Get(r) = r.cTree
                    override x.Set(r,v) = { r with cTree = v }
                    override x.Update(r,f) = { r with cTree = f r.cTree }
                }
            let treeModel =
                { new Lens<Model.Model, TreeViewModel.TModel>() with
                    override x.Get(r) = r.treeModel
                    override x.Set(r,v) = { r with treeModel = v }
                    override x.Update(r,f) = { r with treeModel = f r.treeModel }
                }
