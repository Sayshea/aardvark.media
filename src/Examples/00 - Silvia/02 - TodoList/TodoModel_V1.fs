namespace TodoModel_V1

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

type TodoMessage =
    | AddToTodoList of string
    | DeleteElement of Index
    | AddToCompletedList of string * Index
    | TestMessage of string

[<DomainType>]
type TodoModel =
    {
        todoList : plist<string>
        completed : plist<Index> // I only need the indizes
        testString : string
        testMessage : plist<string>
    }

open System
// design space .....

[<DomainType>]
type MyTask = 
    {
        [<PrimaryKey>]
        name : string
        createDate : DateTime
        completed : bool
    }

type Model2 =
    {
        tasks : hset<MyTask>
    }

//[<DomainType>]
//type MyTask4 = 
//    {
//        createDate : DateTime
//        completed : bool
//    }
//
//type Model4 =
//    {
//        tasks : hmap<string,MyTask>
//    }

//type Key = string
//
//type MyTask2 = 
//    {
//        id : Key
//        name : string
//        createDate : DateTime
//    }
//
//type Model =
//    {
//        pending : hset<MyTask2>
//        completed : hset<Key>
//    }