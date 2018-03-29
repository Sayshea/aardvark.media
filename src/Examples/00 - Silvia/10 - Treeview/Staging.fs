module Staging

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

//let onChange (cb : string -> 'msg) = 
//    onEvent "onchange" ["event.target.value"] (List.head >> Pickler.json.UnPickleOfString >> cb)

let onChangeCheckbox (cb : string -> 'msg) =
    onEvent "onchange" ["event.target.checked"] (List.head >> cb)

//when you have a checkbox and the attribute checked html5 always recognise it, as a checked checkbox, even when you
//set the value to false, so you need to remove the checked attribute
let inputCheckbox (funct : string -> 'msg) (check : bool) =
    input[
        attribute "type" "checkbox"
        (match check with 
            | true -> attribute "checked" "checked"
            | _ -> attribute "value" "")
        onChangeCheckbox (funct)
    ]

//"onchange", 
//AttributeValue.Event { 
//    clientSide = fun send id -> send id ["event.target.value"] + "; event.target.value = '';"
//    serverSide = fun client src args -> 
//        match args with
//            | a :: _ -> Seq.singleton (AddToTodoList (Pickler.json.UnPickleOfString a))
//            | _ -> Seq.empty
//}

let onChangeResetInput (cb : string -> 'msg) = 
    onEvent "onchange" ["event.target.value, event.target.value = ''"] (List.head >> Pickler.json.UnPickleOfString >> cb)