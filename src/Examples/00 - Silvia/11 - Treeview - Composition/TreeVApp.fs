//Remember: When there is a really strange type missmatch, that you really can't explain, then there is maybe a name collision with the
//UI.Primitives - so try another name!!!!

module TreeVApp

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering

open Aardvark.Application

open TreeViewModel

type Message<'userMessage> =
    | Toggle of list<int>
    | UserMessage of 'userMessage

let update ( m : TModel) (msg : Message<'userMessage>) =
    match msg with
    | Toggle path -> 
        match (HSet.contains path m.collapsed) with
        | true -> { m with collapsed = HSet.remove path m.collapsed }
        | false -> { m with collapsed = HSet.add path m.collapsed }
    | UserMessage userMsg -> m

let leafVText a path (ui : 'a -> list<int> -> DomNode<'userMessage>) (m : MTModel) = 
    let leaftext = 
        alist {
            yield Incremental.span (AttributeMap.empty) <| alist {
                yield i [ clazz "file icon";  ] []
                yield ui a path
            }
        }
    Incremental.div (AttributeMap.ofList [ clazz "content" ]) leaftext

let nodeVText a path (ui : 'a -> list<int> -> DomNode<'userMessage>) (m : MTModel) = 
    let nodetext = 
        alist {
            yield div [] [
                yield ui a path
            ]
        }
    Incremental.div (AttributeMap.ofList [clazz "content"]) nodetext

let nodeAttributes (path : list<int>) ( m : MTModel) : AttributeMap<Message<'userMessage>> = 
    amap {
        yield onMouseClick (fun _ -> Toggle path)
        let! checkContains = ASet.contains path m.collapsed
        match checkContains with
        | false -> yield clazz "icon large outline open folder"
        | true -> yield clazz "icon large outline folder"
    } |> AttributeMap.ofAMap

let leafAttributes (path : list<int>) ( m : MTModel) : AttributeMap<Message<'userMessage>> =
    amap{
        yield clazz "list"
        let! checkContains = ASet.contains path m.collapsed
        match checkContains with
        | false -> yield style "visible"
        | true -> yield style "display:none"
    } |> AttributeMap.ofAMap

let rec traverseTree (path:List<int>) (model : InnerTree<'a>) (ui : 'a -> list<int> -> DomNode<'userMessage>) (m : MTModel) : alist<DomNode<Message<'userMessage>>> =
    alist {
        match model with
        | InnerLeaf a -> 
            yield ((leafVText a path ui m) |> UI.map UserMessage)
        | InnerNode (a, c) ->
            let children : alist<DomNode<Message<'userMessage>>> = 
                (List.mapi ( fun i x -> traverseTree (i::path) x ui m) c) 
                |> AList.ofList 
                |> AList.concat

            yield div [ clazz "item" ] [
                Incremental.i (nodeAttributes path m) AList.empty
                div [ clazz "content" ] [
                    div [ clazz "header" ] [(nodeVText a path ui m) |> UI.map UserMessage]
                    Incremental.div (leafAttributes path m) children
                ]
            ]
    }

let view (tree : IMod<InnerTree<'a>>) (ui : 'a -> list<int> -> DomNode<'userMessage>) (m : MTModel) : DomNode<Message<'userMessage>> =
    let tree = 
        tree |> Mod.map (fun tree -> 
            traverseTree [] tree ui m
        )

    require Html.semui (
        Incremental.div AttributeMap.empty <|
             alist {
                let! tree = tree
                yield Incremental.div (AttributeMap.ofList [clazz "ui list"]) tree
            }
    )

