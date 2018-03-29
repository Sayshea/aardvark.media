namespace Model

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open Model

[<AutoOpen>]
module Mutable =

    
    
    type MLeafValue(__initial : Model.LeafValue) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Model.LeafValue> = Aardvark.Base.Incremental.EqModRef<Model.LeafValue>(__initial) :> Aardvark.Base.Incremental.IModRef<Model.LeafValue>
        let _name = ResetMod.Create(__initial.name)
        let _value = ResetMod.Create(__initial.value)
        
        member x.name = _name :> IMod<_>
        member x.value = _value :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.LeafValue) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_name,v.name)
                ResetMod.Update(_value,v.value)
                
        
        static member Create(__initial : Model.LeafValue) : MLeafValue = MLeafValue(__initial)
        static member Update(m : MLeafValue, v : Model.LeafValue) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Model.LeafValue> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module LeafValue =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let name =
                { new Lens<Model.LeafValue, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.name
                    override x.Set(r,v) = { r with name = v }
                    override x.Update(r,f) = { r with name = f r.name }
                }
            let value =
                { new Lens<Model.LeafValue, Microsoft.FSharp.Core.int>() with
                    override x.Get(r) = r.value
                    override x.Set(r,v) = { r with value = v }
                    override x.Update(r,f) = { r with value = f r.value }
                }
    
    
    type MNodeValue(__initial : Model.NodeValue) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Model.NodeValue> = Aardvark.Base.Incremental.EqModRef<Model.NodeValue>(__initial) :> Aardvark.Base.Incremental.IModRef<Model.NodeValue>
        let _name = ResetMod.Create(__initial.name)
        let _unit = ResetMod.Create(__initial.unit)
        
        member x.name = _name :> IMod<_>
        member x.unit = _unit :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.NodeValue) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_name,v.name)
                ResetMod.Update(_unit,v.unit)
                
        
        static member Create(__initial : Model.NodeValue) : MNodeValue = MNodeValue(__initial)
        static member Update(m : MNodeValue, v : Model.NodeValue) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Model.NodeValue> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module NodeValue =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let name =
                { new Lens<Model.NodeValue, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.name
                    override x.Set(r,v) = { r with name = v }
                    override x.Update(r,f) = { r with name = f r.name }
                }
            let unit =
                { new Lens<Model.NodeValue, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.unit
                    override x.Set(r,v) = { r with unit = v }
                    override x.Update(r,f) = { r with unit = f r.unit }
                }
    
    
    type MProperties(__initial : Model.Properties) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Model.Properties> = Aardvark.Base.Incremental.EqModRef<Model.Properties>(__initial) :> Aardvark.Base.Incremental.IModRef<Model.Properties>
        let _isExpanded = ResetMod.Create(__initial.isExpanded)
        let _isSelected = ResetMod.Create(__initial.isSelected)
        
        member x.isExpanded = _isExpanded :> IMod<_>
        member x.isSelected = _isSelected :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.Properties) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_isExpanded,v.isExpanded)
                ResetMod.Update(_isSelected,v.isSelected)
                
        
        static member Create(__initial : Model.Properties) : MProperties = MProperties(__initial)
        static member Update(m : MProperties, v : Model.Properties) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Model.Properties> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Properties =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let isExpanded =
                { new Lens<Model.Properties, Microsoft.FSharp.Core.bool>() with
                    override x.Get(r) = r.isExpanded
                    override x.Set(r,v) = { r with isExpanded = v }
                    override x.Update(r,f) = { r with isExpanded = f r.isExpanded }
                }
            let isSelected =
                { new Lens<Model.Properties, Microsoft.FSharp.Core.bool>() with
                    override x.Get(r) = r.isSelected
                    override x.Set(r,v) = { r with isSelected = v }
                    override x.Update(r,f) = { r with isSelected = f r.isSelected }
                }
    
    
    type MLeafProperties(__initial : Model.LeafProperties) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Model.LeafProperties> = Aardvark.Base.Incremental.EqModRef<Model.LeafProperties>(__initial) :> Aardvark.Base.Incremental.IModRef<Model.LeafProperties>
        let _isSelected = ResetMod.Create(__initial.isSelected)
        
        member x.isSelected = _isSelected :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.LeafProperties) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_isSelected,v.isSelected)
                
        
        static member Create(__initial : Model.LeafProperties) : MLeafProperties = MLeafProperties(__initial)
        static member Update(m : MLeafProperties, v : Model.LeafProperties) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Model.LeafProperties> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module LeafProperties =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let isSelected =
                { new Lens<Model.LeafProperties, Microsoft.FSharp.Core.bool>() with
                    override x.Get(r) = r.isSelected
                    override x.Set(r,v) = { r with isSelected = v }
                    override x.Update(r,f) = { r with isSelected = f r.isSelected }
                }
    [<AbstractClass; System.Runtime.CompilerServices.Extension; StructuredFormatDisplay("{AsString}")>]
    type MTree() =
        abstract member TryUpdate : Model.Tree -> bool
        abstract member AsString : string
        
        static member private CreateValue(__model : Model.Tree) = 
            match __model with
                | Node(value, properties, children) -> MNode(__model, value, properties, children) :> MTree
                | Leaf(value, properties) -> MLeaf(__model, value, properties) :> MTree
        
        static member Create(v : Model.Tree) =
            ResetMod.Create(MTree.CreateValue v) :> IMod<_>
        
        [<System.Runtime.CompilerServices.Extension>]
        static member Update(m : IMod<MTree>, v : Model.Tree) =
            let m = unbox<ResetMod<MTree>> m
            if not (m.GetValue().TryUpdate v) then
                m.Update(MTree.CreateValue v)
    
    and private MNode(__initial : Model.Tree, value : Model.NodeValue, properties : Model.Properties, children : Aardvark.Base.plist<Model.Tree>) =
        inherit MTree()
        
        let mutable __current = __initial
        let _value = MNodeValue.Create(value)
        let _properties = MProperties.Create(properties)
        let _children = ResetMapList(children, (fun _ e -> MTree.Create(e)), (fun (m,e) -> MTree.Update(m, e)))
        member x.value = _value
        member x.properties = _properties
        member x.children = _children :> alist<_>
        
        override x.ToString() = __current.ToString()
        override x.AsString = sprintf "%A" __current
        
        override x.TryUpdate(__model : Model.Tree) = 
            if System.Object.ReferenceEquals(__current, __model) then
                true
            else
                match __model with
                    | Node(value,properties,children) -> 
                        __current <- __model
                        MNodeValue.Update(_value, value)
                        MProperties.Update(_properties, properties)
                        _children.Update(children)
                        true
                    | _ -> false
    
    and private MLeaf(__initial : Model.Tree, value : Model.LeafValue, properties : Model.LeafProperties) =
        inherit MTree()
        
        let mutable __current = __initial
        let _value = MLeafValue.Create(value)
        let _properties = MLeafProperties.Create(properties)
        member x.value = _value
        member x.properties = _properties
        
        override x.ToString() = __current.ToString()
        override x.AsString = sprintf "%A" __current
        
        override x.TryUpdate(__model : Model.Tree) = 
            if System.Object.ReferenceEquals(__current, __model) then
                true
            else
                match __model with
                    | Leaf(value,properties) -> 
                        __current <- __model
                        MLeafValue.Update(_value, value)
                        MLeafProperties.Update(_properties, properties)
                        true
                    | _ -> false
    
    
    [<AutoOpen>]
    module MTreePatterns =
        let (|MNode|MLeaf|) (m : MTree) =
            match m with
            | :? MNode as v -> MNode(v.value,v.properties,v.children)
            | :? MLeaf as v -> MLeaf(v.value,v.properties)
            | _ -> failwith "impossible"
    
    
    
    
    
    
    type MTreeModel(__initial : Model.TreeModel) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<Model.TreeModel> = Aardvark.Base.Incremental.EqModRef<Model.TreeModel>(__initial) :> Aardvark.Base.Incremental.IModRef<Model.TreeModel>
        let _data = MTree.Create(__initial.data)
        let _selected = MList.Create(__initial.selected)
        let _strgDown = ResetMod.Create(__initial.strgDown)
        
        member x.data = _data
        member x.selected = _selected :> alist<_>
        member x.strgDown = _strgDown :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : Model.TreeModel) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MTree.Update(_data, v.data)
                MList.Update(_selected, v.selected)
                ResetMod.Update(_strgDown,v.strgDown)
                
        
        static member Create(__initial : Model.TreeModel) : MTreeModel = MTreeModel(__initial)
        static member Update(m : MTreeModel, v : Model.TreeModel) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<Model.TreeModel> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module TreeModel =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let data =
                { new Lens<Model.TreeModel, Model.Tree>() with
                    override x.Get(r) = r.data
                    override x.Set(r,v) = { r with data = v }
                    override x.Update(r,f) = { r with data = f r.data }
                }
            let selected =
                { new Lens<Model.TreeModel, Aardvark.Base.plist<Microsoft.FSharp.Collections.List<Aardvark.Base.Index>>>() with
                    override x.Get(r) = r.selected
                    override x.Set(r,v) = { r with selected = v }
                    override x.Update(r,f) = { r with selected = f r.selected }
                }
            let strgDown =
                { new Lens<Model.TreeModel, Microsoft.FSharp.Core.bool>() with
                    override x.Get(r) = r.strgDown
                    override x.Set(r,v) = { r with strgDown = v }
                    override x.Update(r,f) = { r with strgDown = f r.strgDown }
                }
