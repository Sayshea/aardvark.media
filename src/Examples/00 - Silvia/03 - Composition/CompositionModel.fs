namespace CompositionModel 

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

open NumericModel
open NumericControl

[<DomainType>]
type VectorModel = { 
    x : NumericModel.Model
    y : NumericModel.Model
    z : NumericModel.Model
}