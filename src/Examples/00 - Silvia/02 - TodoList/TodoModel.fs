namespace TodoModel

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open System

type MyTask = 
    {
        name : string
        createDate : DateTime
        completed : bool
    }

type TodoMessage =
    | AddToTodoList of string
    | DeleteElement of string
    | AddToCompletedList of string * string * MyTask

// design space .....

[<DomainType>]
type TaskList =
    {
        //tasks : hset<MyTask>
        tasks : hmap<string, MyTask>
        activeCount : string
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