module Staging

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

//on change event for a Checkbox, it needs to listen to the event.target.checked
let onChangeCheckbox (cb : string -> 'msg) =
    onEvent "onchange" ["event.target.checked"] (List.head >> cb)

//generates the checkbox and sets the checked attribute
let inputCheckbox (funct : string -> 'msg) (check : bool) =
    match check with
    | true ->
        input [
            attribute "type" "checkbox"
            attribute "checked" "checked"
            onChangeCheckbox (funct)        
        ]
    | false ->
        input [
            attribute "type" "checkbox"
            onChangeCheckbox (funct)        
        ]

//when the focus of the input gets lost, it removes the entry and generate a message
let onChangeResetInput (cb : string -> 'msg) = 
    onEvent "onchange" ["event.target.value, event.target.value = ''"] (List.head >> Pickler.json.UnPickleOfString >> cb)