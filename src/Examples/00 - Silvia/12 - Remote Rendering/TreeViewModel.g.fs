namespace TreeViewModel

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open TreeViewModel

[<AutoOpen>]
module Mutable =

    
    
    type MTModel(__initial : TreeViewModel.TModel) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<TreeViewModel.TModel> = Aardvark.Base.Incremental.EqModRef<TreeViewModel.TModel>(__initial) :> Aardvark.Base.Incremental.IModRef<TreeViewModel.TModel>
        let _collapsed = MSet.Create(__initial.collapsed)
        
        member x.collapsed = _collapsed :> aset<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : TreeViewModel.TModel) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MSet.Update(_collapsed, v.collapsed)
                
        
        static member Create(__initial : TreeViewModel.TModel) : MTModel = MTModel(__initial)
        static member Update(m : MTModel, v : TreeViewModel.TModel) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<TreeViewModel.TModel> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module TModel =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let collapsed =
                { new Lens<TreeViewModel.TModel, Aardvark.Base.hset<Microsoft.FSharp.Collections.list<Microsoft.FSharp.Core.int>>>() with
                    override x.Get(r) = r.collapsed
                    override x.Set(r,v) = { r with collapsed = v }
                    override x.Update(r,f) = { r with collapsed = f r.collapsed }
                }
