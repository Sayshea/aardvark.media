namespace TodoModel_V1

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open TodoModel_V1

[<AutoOpen>]
module Mutable =

    
    
    type MTodoModel(__initial : TodoModel_V1.TodoModel) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<TodoModel_V1.TodoModel> = Aardvark.Base.Incremental.EqModRef<TodoModel_V1.TodoModel>(__initial) :> Aardvark.Base.Incremental.IModRef<TodoModel_V1.TodoModel>
        let _todoList = MList.Create(__initial.todoList)
        let _completed = MList.Create(__initial.completed)
        let _testString = ResetMod.Create(__initial.testString)
        let _testMessage = MList.Create(__initial.testMessage)
        
        member x.todoList = _todoList :> alist<_>
        member x.completed = _completed :> alist<_>
        member x.testString = _testString :> IMod<_>
        member x.testMessage = _testMessage :> alist<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : TodoModel_V1.TodoModel) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                MList.Update(_todoList, v.todoList)
                MList.Update(_completed, v.completed)
                ResetMod.Update(_testString,v.testString)
                MList.Update(_testMessage, v.testMessage)
                
        
        static member Create(__initial : TodoModel_V1.TodoModel) : MTodoModel = MTodoModel(__initial)
        static member Update(m : MTodoModel, v : TodoModel_V1.TodoModel) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<TodoModel_V1.TodoModel> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module TodoModel =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let todoList =
                { new Lens<TodoModel_V1.TodoModel, Aardvark.Base.plist<Microsoft.FSharp.Core.string>>() with
                    override x.Get(r) = r.todoList
                    override x.Set(r,v) = { r with todoList = v }
                    override x.Update(r,f) = { r with todoList = f r.todoList }
                }
            let completed =
                { new Lens<TodoModel_V1.TodoModel, Aardvark.Base.plist<Aardvark.Base.Index>>() with
                    override x.Get(r) = r.completed
                    override x.Set(r,v) = { r with completed = v }
                    override x.Update(r,f) = { r with completed = f r.completed }
                }
            let testString =
                { new Lens<TodoModel_V1.TodoModel, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.testString
                    override x.Set(r,v) = { r with testString = v }
                    override x.Update(r,f) = { r with testString = f r.testString }
                }
            let testMessage =
                { new Lens<TodoModel_V1.TodoModel, Aardvark.Base.plist<Microsoft.FSharp.Core.string>>() with
                    override x.Get(r) = r.testMessage
                    override x.Set(r,v) = { r with testMessage = v }
                    override x.Update(r,f) = { r with testMessage = f r.testMessage }
                }
    
    
    type MMyTask(__initial : TodoModel_V1.MyTask) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<TodoModel_V1.MyTask> = Aardvark.Base.Incremental.EqModRef<TodoModel_V1.MyTask>(__initial) :> Aardvark.Base.Incremental.IModRef<TodoModel_V1.MyTask>
        let _name = ResetMod.Create(__initial.name)
        let _createDate = ResetMod.Create(__initial.createDate)
        let _completed = ResetMod.Create(__initial.completed)
        
        member x.name = _name :> IMod<_>
        member x.createDate = _createDate :> IMod<_>
        member x.completed = _completed :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : TodoModel_V1.MyTask) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_name,v.name)
                ResetMod.Update(_createDate,v.createDate)
                ResetMod.Update(_completed,v.completed)
                
        
        static member Create(__initial : TodoModel_V1.MyTask) : MMyTask = MMyTask(__initial)
        static member Update(m : MMyTask, v : TodoModel_V1.MyTask) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<TodoModel_V1.MyTask> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module MyTask =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let name =
                { new Lens<TodoModel_V1.MyTask, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.name
                    override x.Set(r,v) = { r with name = v }
                    override x.Update(r,f) = { r with name = f r.name }
                }
            let createDate =
                { new Lens<TodoModel_V1.MyTask, System.DateTime>() with
                    override x.Get(r) = r.createDate
                    override x.Set(r,v) = { r with createDate = v }
                    override x.Update(r,f) = { r with createDate = f r.createDate }
                }
            let completed =
                { new Lens<TodoModel_V1.MyTask, Microsoft.FSharp.Core.bool>() with
                    override x.Get(r) = r.completed
                    override x.Set(r,v) = { r with completed = v }
                    override x.Update(r,f) = { r with completed = f r.completed }
                }
