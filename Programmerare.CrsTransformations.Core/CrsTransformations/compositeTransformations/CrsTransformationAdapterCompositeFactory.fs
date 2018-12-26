namespace Programmerare.CrsTransformations.CompositeTransformations

open System.Collections.Generic
open Programmerare.CrsTransformations
(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")
*)

///<summary>
///Factory methods creating 'Composite' implementations of the adapter interface.
///</summary>
[<Sealed>]
type CrsTransformationAdapterCompositeFactory private
    (
        defaultListToUseForFactoryMethodsWithoutParameters: IList<ICrsTransformationAdapter>,
        crsTransformationAdapterLeafFactory: CrsTransformationAdapterLeafFactory
    ) =

    static let ThrowExceptionIfNoKnownInstancesAreAvailable
        (
            adapters: IList<ICrsTransformationAdapter>
        ) : unit =
        if (adapters.Count < 1) then
            failwith "No known CRS transformation implementation was found"

    let _instancesOfAllKnownAvailableImplementationsLazyLoaded: Lazy<IList<ICrsTransformationAdapter>> =
        lazy (
            let list = crsTransformationAdapterLeafFactory.GetInstancesOfAllImplementations()
            ThrowExceptionIfNoKnownInstancesAreAvailable(list)            
            list
        )

    let functionReturningListToUseForFactoryMethodsWithoutParameters: unit -> IList<ICrsTransformationAdapter> = 
        if(isNull defaultListToUseForFactoryMethodsWithoutParameters || defaultListToUseForFactoryMethodsWithoutParameters.Count = 0) then
            fun () -> _instancesOfAllKnownAvailableImplementationsLazyLoaded.Force()
        else
            fun () -> defaultListToUseForFactoryMethodsWithoutParameters

    // ----------------------------------------------
    // Two Median factory methods:

    ///<summary>
    ///Creates a 'composite' implementation by first trying to instantiate
    ///all known 'leaf' implementations available at the class path,
    ///and then pass those to the constructor of the 'composite' 
    ///through the overloaded method with the same name but receiving a list parameter. 
    ///</summary>
    member this.CreateCrsTransformationMedian(): CrsTransformationAdapterComposite =
        this.CreateCrsTransformationMedian (functionReturningListToUseForFactoryMethodsWithoutParameters())

    ///<summary>
    ///Creates a 'composite' implementation by passing 
    ///a list of 'leaf' implementations to the constructor of the 'composite'.
    ///The created composite is used when the median value is desired as
    ///the output coordinate. However, the average will also be available
    ///almost as easily by getting access to a statistics object from the result object.
    ///</summary>
    ///<param name="list">
    ///a list of implementations which will very probably be 'leafs' although 
    ///this is not strictly enforced by the type system, so in theory
    ///the list could be composites i.e. you might try to nest 
    ///composites with composites (but it is not obvious why anyone would want to do that)
    ///</param>
    member private this.CreateCrsTransformationMedian(list: IList<ICrsTransformationAdapter>): CrsTransformationAdapterComposite =
        CrsTransformationAdapterCompositeMedianValue.Create(list) :> CrsTransformationAdapterComposite
    // ----------------------------------------------

    // Two Average factory methods:

    ///<summary>
    ///Please see the documentation for the factory methods creating a median composite.
    ///The only difference is that the average will be returned instead of the median.
    ///</summary>
    member this.CreateCrsTransformationAverage(): CrsTransformationAdapterComposite =
        this.CreateCrsTransformationAverage(functionReturningListToUseForFactoryMethodsWithoutParameters())

    ///<summary>
    ///Please see the documentation for the factory methods creating a median composite.
    ///The only difference is that the average will be returned instead of the median.
    ///</summary>
    member private this.CreateCrsTransformationAverage(list: IList<ICrsTransformationAdapter>): CrsTransformationAdapterComposite =
        CrsTransformationAdapterCompositeAverageValue.Create(list) :> CrsTransformationAdapterComposite
    // ----------------------------------------------

    // Two FirstSuccess factory methods:

    ///<summary>
    ///Please see documentation for the overloaded method.
    ///</summary>
    member this.CreateCrsTransformationFirstSuccess(): CrsTransformationAdapterComposite =
        this.CreateCrsTransformationFirstSuccess (functionReturningListToUseForFactoryMethodsWithoutParameters())

    ///<summary>
    ///Please also see the documentation for the factory methods creating a median composite.
    ///The difference is that the 'FirstSuccess' will not try
    ///to use all leaf implementations but will only continue with the next
    ///leaf implementation until a succesful result has been found.
    ///</summary>
    member private this.CreateCrsTransformationFirstSuccess(list: IList<ICrsTransformationAdapter>): CrsTransformationAdapterComposite =
        CrsTransformationAdapterCompositeFirstSuccess.Create(list) :> CrsTransformationAdapterComposite

    // ----------------------------------------------


    ///<summary>
    ///Similar to the factory method creating an implementation to calculate the average.
    ///The difference is that this method with weighted average can be used when you want to assign
    ///certain adaptee implementations different weights, i.e. when you feel that you have a reason 
    ///to believe that certain leaf implementations tend to provide better results than others.
    ///</summary>
    member this.CreateCrsTransformationWeightedAverage
        (
            weightedCrsTransformationAdapters: IList<CrsTransformationAdapterWeight>
        ): CrsTransformationAdapterComposite =
            CrsTransformationAdapterCompositeWeightedAverageValue.Create(weightedCrsTransformationAdapters) :> CrsTransformationAdapterComposite

            // Another feature to maybe implement:
            // Configuration paramters e.g. dictionary 
            // with strings and e.g. using the "ShortName" as prefix 
            // for the adapter it should apply to.
            // Example:
            // "ProjNet4GeoAPI.CrsCachingStrategy" = "NO_CACHING"
            // which should be the same as explicitly 
            // creating a CrsTransformationAdapterProjNet4GeoAPI
            // and then invoke:
            // adapter.SetCrsCachingStrategy(CrsCachingStrategy.NO_CACHING)
            // and then pass it as one of the adapters in the list
            // to the constructory receiving a list

    static member Create
        (
            defaultListToUseForFactoryMethodsWithoutParameters: IList<ICrsTransformationAdapter>
        ) =
        CrsTransformationAdapterCompositeFactory(
            defaultListToUseForFactoryMethodsWithoutParameters, 
            CrsTransformationAdapterLeafFactory.Create()
        )

    static member Create() =
        // The constructor (used below with "null") 
        // should be C# friendly 
        // i.e. use types such as IList which can be null
        // so null is used below (as alternative to empty list) 
        // as a signal that all available implementatiosn should be used
        // instead of using a list specified by the user.
        CrsTransformationAdapterCompositeFactory(
            null, 
            CrsTransformationAdapterLeafFactory.Create()
        )

    static member Create
        (
            crsTransformationAdapterLeafFactory: CrsTransformationAdapterLeafFactory
        ) =
        CrsTransformationAdapterCompositeFactory(
            null,
            crsTransformationAdapterLeafFactory
        )

    static member Create
        (
            defaultListToUseForFactoryMethodsWithoutParameters: IList<ICrsTransformationAdapter>,
            crsTransformationAdapterLeafFactory: CrsTransformationAdapterLeafFactory
        ) =
        CrsTransformationAdapterCompositeFactory(
            defaultListToUseForFactoryMethodsWithoutParameters,
            crsTransformationAdapterLeafFactory
        )