namespace Programmerare.CrsTransformations.CompositeTransformations

open Programmerare.CrsTransformations

(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")
*)

///<summary>
///<para>
///An instance of this class defines how much relative weight
///a certain adapter implementation should have.
///</para>
///<para/>
///<para>
///A list of these instances should be passed to the factory 
///method for a composite object used for returning a weighted average 
///of the longitude and latitude values originating from different leaf 
///adapter implementations doing coordinate transformations.
///</para>
///</summary>
type CrsTransformationAdapterWeight private
    (
        crsTransformationAdapter: ICrsTransformationAdapter,
        weight: double
    ) =

    do
        if isNull crsTransformationAdapter then
            nullArg "crsTransformationAdapter"
        if(weight <= 0.0) then
            invalidArg "weight" ("The weight value must be positive value. It does not make sense to try using negative values, and there would be useless with a zero value. The weight value was: " + weight.ToString()  + " for the adapter " + crsTransformationAdapter.ShortNameOfImplementation)

    member x.Weight = weight
    member x.CrsTransformationAdapter = crsTransformationAdapter

    static member internal _Create
        (
            crsTransformationAdapter: ICrsTransformationAdapter,
            weight: double
        ) = 
            CrsTransformationAdapterWeight(crsTransformationAdapter, weight)