namespace CompositionModel

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open CompositionModel

[<AutoOpen>]
module Mutable =

    [<AbstractClass; StructuredFormatDisplay("{AsString}")>]
    type MTModel<'va,'na>() = 
        abstract member tree : Aardvark.Base.Incremental.aset<'na>
        abstract member AsString : string
    
    
    and private MTModelD<'a,'ma,'va>(__initial : CompositionModel.TModel<'a>, __ainit : 'a -> 'ma, __aupdate : 'ma * 'a -> unit, __aview : 'ma -> 'va) =
        inherit MTModel<'va,'va>()
        let mutable __current : Aardvark.Base.Incremental.IModRef<CompositionModel.TModel<'a>> = Aardvark.Base.Incremental.EqModRef<CompositionModel.TModel<'a>>(__initial) :> Aardvark.Base.Incremental.IModRef<CompositionModel.TModel<'a>>
        let _tree = MSet.Create(unbox, __initial.tree, (fun v -> __ainit(v)), (fun (m,v) -> __aupdate(m, v)), (fun v -> __aview(v)))
        
        override x.tree = _tree :> aset<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : CompositionModel.TModel<'a>) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MSet.Update(_tree, v.tree)
                
        
        static member Update(m : MTModelD<'a,'ma,'va>, v : CompositionModel.TModel<'a>) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        override x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<CompositionModel.TModel<'a>> with
            member x.Update v = x.Update v
    
    and private MTModelV<'a>(__initial : CompositionModel.TModel<'a>) =
        inherit MTModel<IMod<'a>,'a>()
        let mutable __current : Aardvark.Base.Incremental.IModRef<CompositionModel.TModel<'a>> = Aardvark.Base.Incremental.EqModRef<CompositionModel.TModel<'a>>(__initial) :> Aardvark.Base.Incremental.IModRef<CompositionModel.TModel<'a>>
        let _tree = MSet.Create(__initial.tree)
        
        override x.tree = _tree :> aset<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : CompositionModel.TModel<'a>) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MSet.Update(_tree, v.tree)
                
        
        static member Update(m : MTModelV<'a>, v : CompositionModel.TModel<'a>) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        override x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<CompositionModel.TModel<'a>> with
            member x.Update v = x.Update v
    
    and [<AbstractClass; Sealed>] MTModel private() =
        static member Create<'a,'ma,'va>(__initial : CompositionModel.TModel<'a>, __ainit : 'a -> 'ma, __aupdate : 'ma * 'a -> unit, __aview : 'ma -> 'va) : MTModel<'va,'va> = MTModelD<'a,'ma,'va>(__initial, __ainit, __aupdate, __aview) :> MTModel<'va,'va>
        static member Create<'a>(__initial : CompositionModel.TModel<'a>) : MTModel<IMod<'a>,'a> = MTModelV<'a>(__initial) :> MTModel<IMod<'a>,'a>
        static member Update<'a,'xva,'xna>(m : MTModel<'xva,'xna>, v : CompositionModel.TModel<'a>) : unit = 
            match m :> obj with
            | :? IUpdatable<CompositionModel.TModel<'a>> as m -> m.Update(v)
            | _ -> failwith "cannot update"
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module TModel =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let tree<'a> =
                { new Lens<CompositionModel.TModel<'a>, Aardvark.Base.hset<'a>>() with
                    override x.Get(r) = r.tree
                    override x.Set(r,v) = { r with tree = v }
                    override x.Update(r,f) = { r with tree = f r.tree }
                }
