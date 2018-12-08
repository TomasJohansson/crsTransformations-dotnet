namespace Programmerare.CrsTransformations.CompositeTransformations
open Programmerare.CrsTransformations
(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")


 * An instance of this class defines how much relative weight
 * a certain adapter implementation should have.
 * 
 * A list of these instances should be passed to the factory 
 * method for a composite object used for returning a weighted average 
 * of the longitude and latitude values originating from different leaf 
 * adapter implementations doing coordinate transformations.

 * @see CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage
 * @param crsTransformationAdapter an object implementing the interface CrsTransformationAdapter
 * @param weight the relative weight value to assign for the adapter specified by the adapter parameter
*)
type CrsTransformationAdapterWeight private
    (
        crsTransformationAdapter: ICrsTransformationAdapter,
        weight: double
    ) =

    static let crsTransformationAdapterLeafFactory = CrsTransformationAdapterLeafFactory.Create()

    do
        if isNull crsTransformationAdapter then
            nullArg "crsTransformationAdapter"
        if(weight <= 0.0) then
            invalidArg "weight" ("The weight value must be positive value. It does not make sense to try using negative values, and there would be useless with a zero value. The weight value was: " + weight.ToString()  + " for the adapter " + crsTransformationAdapter.ShortNameOfImplementation)

    member x.Weight = weight
    member x.CrsTransformationAdapter = crsTransformationAdapter

    (*
    * @param crsTransformationAdapterClassName the full class name (i.e. including the package name)
    *  of a class which must implement the interface CrsTransformationAdapter
    * @param weight the relative weight value to assign for the adapter specified by the string parameter 
    *)
    static member CreateFromStringWithFullClassNameForImplementation
        (
            crsTransformationAdapterClassName: string,
            weight: double
        ): CrsTransformationAdapterWeight =
        let crsTransformationAdapter = crsTransformationAdapterLeafFactory.GetCrsTransformationAdapter(crsTransformationAdapterClassName)
        CrsTransformationAdapterWeight(crsTransformationAdapter, weight)

    (*
    * @param crsTransformationAdapter an object implementing the interface CrsTransformationAdapter
    * @param weight the relative weight value to assign for the adapter specified by the adapter parameter 
    *)        
    static member CreateFromInstance
        (
            crsTransformationAdapter: ICrsTransformationAdapter,
            weight: double
        ): CrsTransformationAdapterWeight =
        CrsTransformationAdapterWeight(crsTransformationAdapter, weight)
