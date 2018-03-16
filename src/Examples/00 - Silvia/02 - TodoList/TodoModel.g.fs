namespace TodoModel

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open TodoModel

[<AutoOpen>]
module Mutable =

    
    
    type MTaskList(__initial : TodoModel.TaskList) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<TodoModel.TaskList> = Aardvark.Base.Incremental.EqModRef<TodoModel.TaskList>(__initial) :> Aardvark.Base.Incremental.IModRef<TodoModel.TaskList>
        let _tasks = MMap.Create(__initial.tasks)
        let _activeCount = ResetMod.Create(__initial.activeCount)
        
        member x.tasks = _tasks :> amap<_,_>
        member x.activeCount = _activeCount :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : TodoModel.TaskList) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MMap.Update(_tasks, v.tasks)
                ResetMod.Update(_activeCount,v.activeCount)
                
        
        static member Create(__initial : TodoModel.TaskList) : MTaskList = MTaskList(__initial)
        static member Update(m : MTaskList, v : TodoModel.TaskList) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<TodoModel.TaskList> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module TaskList =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let tasks =
                { new Lens<TodoModel.TaskList, Aardvark.Base.hmap<Microsoft.FSharp.Core.string,TodoModel.MyTask>>() with
                    override x.Get(r) = r.tasks
                    override x.Set(r,v) = { r with tasks = v }
                    override x.Update(r,f) = { r with tasks = f r.tasks }
                }
            let activeCount =
                { new Lens<TodoModel.TaskList, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.activeCount
                    override x.Set(r,v) = { r with activeCount = v }
                    override x.Update(r,f) = { r with activeCount = f r.activeCount }
                }
