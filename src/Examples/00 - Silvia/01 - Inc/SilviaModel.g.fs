namespace SilviaModel

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open SilviaModel

[<AutoOpen>]
module Mutable =

    
    
    type MSModel(__initial : SilviaModel.SModel) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<SilviaModel.SModel> = Aardvark.Base.Incremental.EqModRef<SilviaModel.SModel>(__initial) :> Aardvark.Base.Incremental.IModRef<SilviaModel.SModel>
        let _inputString = ResetMod.Create(__initial.inputString)
        let _counter = ResetMod.Create(__initial.counter)
        let _inputString1 = ResetMod.Create(__initial.inputString1)
        let _inputString2 = ResetMod.Create(__initial.inputString2)
        let _inputString3 = ResetMod.Create(__initial.inputString3)
        let _inputStringUnteres = ResetMod.Create(__initial.inputStringUnteres)
        let _inputList = MList.Create(__initial.inputList)
        
        member x.inputString = _inputString :> IMod<_>
        member x.counter = _counter :> IMod<_>
        member x.inputString1 = _inputString1 :> IMod<_>
        member x.inputString2 = _inputString2 :> IMod<_>
        member x.inputString3 = _inputString3 :> IMod<_>
        member x.inputStringUnteres = _inputStringUnteres :> IMod<_>
        member x.inputList = _inputList :> alist<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : SilviaModel.SModel) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_inputString,v.inputString)
                ResetMod.Update(_counter,v.counter)
                ResetMod.Update(_inputString1,v.inputString1)
                ResetMod.Update(_inputString2,v.inputString2)
                ResetMod.Update(_inputString3,v.inputString3)
                ResetMod.Update(_inputStringUnteres,v.inputStringUnteres)
                MList.Update(_inputList, v.inputList)
                
        
        static member Create(__initial : SilviaModel.SModel) : MSModel = MSModel(__initial)
        static member Update(m : MSModel, v : SilviaModel.SModel) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<SilviaModel.SModel> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module SModel =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let inputString =
                { new Lens<SilviaModel.SModel, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.inputString
                    override x.Set(r,v) = { r with inputString = v }
                    override x.Update(r,f) = { r with inputString = f r.inputString }
                }
            let counter =
                { new Lens<SilviaModel.SModel, Microsoft.FSharp.Core.int>() with
                    override x.Get(r) = r.counter
                    override x.Set(r,v) = { r with counter = v }
                    override x.Update(r,f) = { r with counter = f r.counter }
                }
            let inputString1 =
                { new Lens<SilviaModel.SModel, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.inputString1
                    override x.Set(r,v) = { r with inputString1 = v }
                    override x.Update(r,f) = { r with inputString1 = f r.inputString1 }
                }
            let inputString2 =
                { new Lens<SilviaModel.SModel, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.inputString2
                    override x.Set(r,v) = { r with inputString2 = v }
                    override x.Update(r,f) = { r with inputString2 = f r.inputString2 }
                }
            let inputString3 =
                { new Lens<SilviaModel.SModel, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.inputString3
                    override x.Set(r,v) = { r with inputString3 = v }
                    override x.Update(r,f) = { r with inputString3 = f r.inputString3 }
                }
            let inputStringUnteres =
                { new Lens<SilviaModel.SModel, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.inputStringUnteres
                    override x.Set(r,v) = { r with inputStringUnteres = v }
                    override x.Update(r,f) = { r with inputStringUnteres = f r.inputStringUnteres }
                }
            let inputList =
                { new Lens<SilviaModel.SModel, Aardvark.Base.plist<Microsoft.FSharp.Core.string>>() with
                    override x.Get(r) = r.inputList
                    override x.Set(r,v) = { r with inputList = v }
                    override x.Update(r,f) = { r with inputList = f r.inputList }
                }
