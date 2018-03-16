namespace CompositionModel 

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

open NumericModel
open NumericControl

[<DomainType>]
type VectorModel = { 
    vectorList : plist<NumericModel.Model>
    numDim : int
}