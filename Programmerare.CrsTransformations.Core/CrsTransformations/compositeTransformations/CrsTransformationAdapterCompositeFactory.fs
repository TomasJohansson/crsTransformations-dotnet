namespace Programmerare.CrsTransformations.CompositeTransformations
open System.Collections.Generic
open Programmerare.CrsTransformations
(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")

 * Factory methods creating 'Composite' implementations of the adapter interface.
*)
[<AbstractClass; Sealed>]
type CrsTransformationAdapterCompositeFactory =

    static member private throwExceptionIfNoKnownInstancesAreAvailable
        (
            adapters: IList<ICrsTransformationAdapter>
        ) : unit =
        if (adapters.Count < 1) then
            failwith "No known CRS transformation implementation was found"
        
    // ----------------------------------------------
    // Two Median factory methods:

    (*
     * Creates a 'composite' implementation by first trying to instantiate
     * all known 'leaf' implementations available at the class path,
     * and then pass those to the constructor of the 'composite' 
     * through the overloaded method with the same name but receiving a list parameter. 
     * @see createCrsTransformationMedian
     *)
    static member CreateCrsTransformationMedian(): CrsTransformationAdapterComposite =
        let list = CrsTransformationAdapterLeafFactory.GetInstancesOfAllKnownAvailableImplementations()
        CrsTransformationAdapterCompositeFactory.throwExceptionIfNoKnownInstancesAreAvailable list
        CrsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian list

    (*
     * Creates a 'composite' implementation by passing 
     * a list of 'leaf' implementations to the constructor of the 'composite'.
     * The created composite is used when the median value is desired as
     * the output coordinate. However, the average will also be available
     * almost as easily by getting access to a statistics object from the result object.
     * @param list a list of implementations which will very probably be 'leafs' although 
     *      this is not strictly enforced by the type system, so in theory
     *      the list could be composites i.e. you might try to nest 
     *      composites with composites (but it is not obvious why anyone would want to do that)
     *)    
    static member CreateCrsTransformationMedian(list: IList<ICrsTransformationAdapter>): CrsTransformationAdapterComposite =
        CrsTransformationAdapterComposite._CreateCrsTransformationAdapterComposite
            (
                CompositeStrategyForMedianValue._CreateCompositeStrategyForMedianValue
                    (
                        list
                    )
            )
    // ----------------------------------------------


    // ----------------------------------------------
    // Two Average factory methods:

    (*
     * Please see the documentation for the factory methods creating a median composite.
     * The only difference is that the average will be returned instead of the median.
     * @see createCrsTransformationMedian 
     *)
    static member CreateCrsTransformationAverage(): CrsTransformationAdapterComposite =
        let list = CrsTransformationAdapterLeafFactory.GetInstancesOfAllKnownAvailableImplementations()
        CrsTransformationAdapterCompositeFactory.throwExceptionIfNoKnownInstancesAreAvailable list
        CrsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage list

    (*
     * Please see the documentation for the factory methods creating a median composite.
     * The only difference is that the average will be returned instead of the median.
     * @see createCrsTransformationMedian
     *)    
    static member CreateCrsTransformationAverage(list: IList<ICrsTransformationAdapter>): CrsTransformationAdapterComposite =
        CrsTransformationAdapterComposite._CreateCrsTransformationAdapterComposite
            (
                CompositeStrategyForAverageValue._CreateCompositeStrategyForAverageValue
                    (
                        list
                    )
            )
    // ----------------------------------------------

    // ----------------------------------------------
    // Two FirstSuccess factory methods:

    (*
     * Please see documentation for the overloaded method.
     * @see createCrsTransformationFirstSuccess
     *)    
    static member CreateCrsTransformationFirstSuccess(): CrsTransformationAdapterComposite =
        let list = CrsTransformationAdapterLeafFactory.GetInstancesOfAllKnownAvailableImplementations()
        CrsTransformationAdapterCompositeFactory.throwExceptionIfNoKnownInstancesAreAvailable list
        CrsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess list

    (*
     * Please also see the documentation for the factory methods creating a median composite.
     * The difference is that the 'FirstSuccess' will not try
     * to use all leaf implementations but will only continue with the next
     * leaf implementation until a succesful result has been found.
     * @see createCrsTransformationMedian
     *)    
    static member CreateCrsTransformationFirstSuccess(list: IList<ICrsTransformationAdapter>): CrsTransformationAdapterComposite =
        CrsTransformationAdapterComposite._CreateCrsTransformationAdapterComposite
            (
                CompositeStrategyForFirstSuccess._CreateCompositeStrategyForFirstSuccess
                    (
                        list
                    )
            )

    // ----------------------------------------------


    (*
     * Similar to the factory method creating an implementation to calculate the average.
     * The difference is that this method with weighted average can be used when you want to assign
     * certain adaptee implementations different weights, i.e. when you feel that you have a reason 
     * to believe that certain leaf implementations tend to provide better results than others.
     * @see createCrsTransformationAverage
     * @see CrsTransformationAdapterWeight
     *)
    static member CreateCrsTransformationWeightedAverage
        (
            weightedCrsTransformationAdapters: IList<CrsTransformationAdapterWeight>
        ): CrsTransformationAdapterComposite =
            CrsTransformationAdapterComposite._CreateCrsTransformationAdapterComposite
                (
                    CompositeStrategyForWeightedAverageValue._CreateCompositeStrategyForWeightedAverageValue(weightedCrsTransformationAdapters)
                )
