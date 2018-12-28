namespace Programmerare.CrsTransformations

open System
open System.Linq
open System.Collections.Generic
open Programmerare.CrsTransformations.Coordinate
open Programmerare.CrsTransformations.Identifier
open Programmerare.CrsTransformations.Utils
(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")

IMPORTANT NOTE:
There are THREE types in this file:
    - ICrsTransformationAdapter
    - CrsTransformationResult
    - CrsTransformationResultStatistic
(regarding the reason: read below)

The above three types in this file are separated with "and" 
( or actually the keyword "and" is used INSTEAD OF "type" 
for the second and third types in this file).
The reason for having these three types in the same file 
is that F# compiles files in a certain order which often 
is not a problem since you indeed usually want to 
avoid bidirectional dependencies. 
In this case I think it is reasonable/natural with some 
mutual dependencies between the three types in this file.
That is the reason for having put them here in the same file
i.e. having them here in the same file solves the problem 
by doing that and by using the keyword "and" between the types.
*)
//--------------------------------------------------------
//Type ICrsTransformationAdapter:
//(the first of the three types in this file)

///<summary>
///<para>
///This adapter interface is the core type of this CRS transformation library.
///It defines six transform methods.
///</para>
///<para/>
///<para>
///Three of those methods will only return a coordinate with the result,
///while the other three will return a result object
///which also contains more information e.g. all the individual 'leaf'
///results if the implementing class was a 'composite'.
///</para>
///<para/>
///<para>
///The difference between the three methods (returning the same type)
///is just the last parameter which is either an integer value
///with an EPSG code (e.g. 4326) or a string (an integer value but also with a "EPSG:"-prefix e.g. "EPSG:4326")
///or an instance of CrsIdentifier (which also is a representation of a CRS such as EPSG:4326).
///</para>
///<para/>
///<para>
///If a transform method without the CrsIdentifier parameter is used then
///the CrsIdentifier will be created by the other transform methods.
///</para>
///<para/>
///<para> 
///In other words, the methods with integer or string parameter are
///convenience methods. If you are going to do many transformations
///you may want to create the CrsIdentifier object once yourself
///and then use a transform method using it as a parameter.
///</para>
///<para/>
///<para>
///The methods 'TransformToCoordinate' can throw exception when the transformation fails.
///The methods 'Transform' should always return a result object rather than throwing an exception.
///If you use a 'Transform' method returning the result object then you should
///check for failures with 'TransformationResult.IsSuccess'.
///</para>
///</summary>
[<AllowNullLiteral>] // C# interoperability
type ICrsTransformationAdapter =
    interface
        // -------------------------------------------------
        // Below are the three methods returning a coordinate object:

        ///<summary>
        ///Transforms a coordinate to a coordinate
        ///in another coordinate reference system if possible
        ///but may throw an exception if the transformation fails.
        ///The integer parameter is an EPSG number such as 4326 for the CRS 'WGS84'.
        ///</summary>
        abstract member TransformToCoordinate : CrsCoordinate * int -> CrsCoordinate

        ///<summary>
        ///Transforms a coordinate to a coordinate
        ///in another coordinate reference system if possible
        ///but may throw an exception if the transformation fails.
        ///The string parameter is an EPSG code such as "EPSG:4326" for the CRS 'WGS84'.
        ///</summary>
        abstract member TransformToCoordinate : CrsCoordinate * string -> CrsCoordinate

        ///<summary>
        ///Transforms a coordinate to a coordinate
        ///in another coordinate reference system if possible
        ///but may throw an exception if the transformation fails.
        ///</summary>
        abstract member TransformToCoordinate : CrsCoordinate * CrsIdentifier -> CrsCoordinate
        
        // Above are the three methods returning a coordinate object.
        // -------------------------------------------------
        // Below are the three methods returning a transformation result object:

        ///<summary>
        ///Transforms a coordinate to another coordinate reference system.
        ///The method should never throw an exception but instead one of the methods
        ///in the result object should be used to check for failure.
        ///The integer parameter is an EPSG number such as 4326 for the CRS 'WGS84'.
        ///</summary>
        abstract member Transform : CrsCoordinate * int -> CrsTransformationResult

        ///<summary>
        ///Transforms a coordinate to another coordinate reference system.
        ///The method should never throw an exception but instead one of the methods
        ///in the result object should be used to check for failure.
        ///The string parameter is an EPSG code such as "EPSG:4326" for the CRS 'WGS84'.
        ///</summary>
        abstract member Transform : CrsCoordinate * string -> CrsTransformationResult

        ///<summary>
        ///Transforms a coordinate to another coordinate reference system.
        ///The method should never throw an exception but instead one of the methods
        ///in the result object should be used to check for failure.
        ///</summary>
        abstract member Transform : CrsCoordinate * CrsIdentifier -> CrsTransformationResult

        // Above are the three methods returning a transformation result object.
        // -------------------------------------------------

        ///<value>
        ///Should normally simply return the full class name (including the package name), 
        ///but when implementing test doubles (e.g. using the framework "Moq")
        ///then the method should be implemented by defining different names
        ///to simulate that different classes (implementations)
        ///should have different weights.
        ///</value>
        abstract member LongNameOfImplementation : string

        ///<value>
        ///The unique suffix part of the class name
        ///i.e. the class name without a prefix which is common
        ///for many implementations.
        ///</value>
        abstract member ShortNameOfImplementation : string

        ///<summary>
        /// See documentation of <see cref="CrsTransformationAdapteeType"/>
        ///</summary>
        abstract member AdapteeType : CrsTransformationAdapteeType

        ///<value>
        ///true if the implementation is a 'composite'
        ///but false if it is a 'leaf' implementation
        ///</value>
        abstract member IsComposite : bool

        ///<returns>
        ///a list of children/leafs when the implementation 
        ///is a 'composite' but if the implementation is a 'leaf'
        ///then an empty list should be returned.
        ///</returns>
        abstract member GetTransformationAdapterChildren : unit -> IList<ICrsTransformationAdapter>

    end

// NOTE THAT THERE ARE MORE THAN ONE TYPES IN THIS FILE
// (one type above and two below)

// ---------------------------------------
// ---------------------------------------
// ---------------------------------------

// NOTE THAT THERE ARE MORE THAN ONE TYPES IN THIS FILE
// (one type above and two below)

/// --------------------------------------------------------
/// Type CrsTransformationResult:
/// (the SECOND of the three types in this file)

///<summary>
///<para>
///This class is used as result type from three of the transform methods 
///of the adapter interface (i.e. the first type above in this file).
///</para>
///<para/>
///<para>
///Normally you define a F# class with "type" as below:
///"type CrsTransformationResult" (instead of "and CrsTransformationResult")
///However, to make it possible for the types to refer to each other
///they have been put here in the same file 
///and therefore uses the keyword "and" as below instead of "type"
///</para>
///</summary>
and CrsTransformationResult private
    (
        // https://stackoverflow.com/questions/15121814/documenting-f-code/15138597
        // > "...there is no way of documenting the implicit constructor with an XML comment"
        inputCoordinate: CrsCoordinate,
        outputCoordinate: CrsCoordinate, //[<Optional; DefaultParameterValue(null:CrsCoordinate)>] outputCoordinate: CrsCoordinate,
        exceptionOrNull: Exception,
        isSuccess: bool,
        
        crsTransformationAdapterResultSource: ICrsTransformationAdapter,
        // Note that the above parameter is a mutual dependency
        // since ICrsTransformationAdapter also uses the CrsTransformationResult
        // as result type for Transform methods

        // Also note that the below is a mutual dependency
        // since it also uses a dependency to CrsTransformationResult
        crsTransformationResultStatistic: CrsTransformationResultStatistic
    ) =
    class
        do
            // The below exceptions should not occurr.
            // If they do then there is a bug that should be fixed.
            if( isSuccess && (isNull outputCoordinate) ) then
                invalidArg "isSuccess" "Unvalid object construction. If success then output coordinate should NOT be null"
            if( not(isSuccess) && not(isNull outputCoordinate) ) then
                invalidArg "isSuccess" "Unvalid object construction. If NOT success then output coordinate SHOULD be null"
            if( not(isNull exceptionOrNull)) then // if there was an exception then should NOT be success and NOT any resulting coordinate !
                if(isSuccess || not(isNull outputCoordinate)) then
                    invalidArg "isSuccess" "Unvalid object construction. If exception then output coordinate should be null and success should be false"
            // From the Kotlin project's implementation:
            //        if(transformationResultChildren == null || transformationResultChildren.size <= 0) {
            //            // SHOULD be a leaf since no children, i.e. throw exception if Composite
            //            if(crsTransformationAdapterResultSource.isComposite()) {
            //                throw IllegalStateException("Inconsistent result: 'Composite' without 'leafs' (should not be possible)")    
            //            }
            //        }
            //        else { // i.e. size > 0
            //            // SHOULD be a composite since there are children, i.e. throw exception if Leaf
            //            if(!crsTransformationAdapterResultSource.isComposite()) { // Not Composite means it is a Leaf
            //                throw IllegalStateException("Inconsistent result: 'Leaf' with 'children' (should not be possible)")
            //            }            
            //        }
            //    }

        ///<value>The input coordinate used in the transform that return the result object.</value>
        member this.InputCoordinate = inputCoordinate

        ///<summary>
        ///<para>
        ///The coordinate which is the result from the transform.
        ///</para>
        ///<para/>
        ///<para>
        ///Precondition: Verify that the success property returns true before using this accessor.
        ///If it returns false, then an exception will be thrown.
        ///</para>
        ///<para/>
        ///<para>
        ///Depending on the adapter implementation, the output coordinate
        ///can either be a direct result from one specific 'leaf' adaptee implementation
        ///or it can be an aggregated result (i.e. median or average) from
        ///a 'composite' implementation.
        ///</para>
        ///</summary>
        member this.OutputCoordinate 
            with get() = 
                if(not(isSuccess)) then 
                    invalidOp "Pre-condition violated. Coordinate retrieval only allowed if result was success"
                outputCoordinate

        ///<summary>
        ///Either null or an exception depending on whether or not
        ///the transform resulted in an exception being thrown.
        ///</summary>
        member this.Exception = exceptionOrNull

        ///<value>
        ///True if the transform was successful or false if it failed.
        ///Note that "successful" does not necessarily mean that the
        ///result is correct but at least an exception was not thrown
        ///and the result was not "NaN" (Not a Number).
        ///(i.e. maybe "SeemsToBeSuccess" might be a better property name)
        ///</value>
        member this.IsSuccess = isSuccess

        ///<value>
        ///The adapter which created the result.
        ///It may be useful when a composite adapter is returning a result aggregating many results
        ///and you want to figure out which result originated from which leaf adapter implementation.
        ///</value>
        member this.CrsTransformationAdapterResultSource = crsTransformationAdapterResultSource

        ///<value>
        ///An object with conveniently available aggregating information about the
        ///results for the different implementations, which is useful for composite implementations.
        ///For a leaf implementation this property is not meaningful.
        ///It is a convenience property in the sense that the information provided
        ///can be calculated from client code by iterating the leafs/children of a composite.
        ///</value>
        member this.CrsTransformationResultStatistic = 
            if(this.CrsTransformationAdapterResultSource.IsComposite) then
                crsTransformationResultStatistic
            else
                let list = new List<CrsTransformationResult>([this]) // alternative: new ResizeArray
                CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(list)

            // From the Kotlin project's implementation:
            //    val crsTransformationResultStatistic: CrsTransformationResultStatistic
            //        get() {
            //            return _crsTransformationResultStatisticLazyLoaded
            //        }
            // The Kotlin project's implementation of the above property:
            //    private val _crsTransformationResultStatisticLazyLoaded: CrsTransformationResultStatistic by lazy {
            //        if(_nullableCrsTransformationResultStatistic != null) {
            //            _nullableCrsTransformationResultStatistic
            //        }
            //        else if(this.transformationResultChildren.size == 0) {
            //            CrsTransformationResultStatistic._createCrsTransformationResultStatistic(listOf<CrsTransformationResult>(this))
            //        }
            //        else {
            //            CrsTransformationResultStatistic._createCrsTransformationResultStatistic(transformationResultChildren)
            //        }
            //    }

        ///<returns>
        ///Empty list if the transform implementation is a concrete "Leaf"
        ///implementation, but if it is a composite/aggregating implementation
        ///then all the individual "leaf" results are returned in this list.
        ///</returns>
        member this.GetTransformationResultChildren() = 
            if(this.CrsTransformationAdapterResultSource.IsComposite) then
                crsTransformationResultStatistic.GetAllCrsTransformationResults()
            else
                new List<CrsTransformationResult>() :> IList<CrsTransformationResult>

        ///<summary>
        ///<para>
        ///Convenience method intended for "Composite" implementations
        ///to easy check that more than one implementation (the specified minimum number)
        ///resulted in the same coordinate (within the specified delta value).
        ///</para>
        ///<para/>
        ///<para>
        ///If false is returned then you may choose to retrieve the 
        ///CrsTransformationResultStatistic object to find the details regarding the differences.
        ///</para>
        ///<para/>
        ///<para>
        ///The method is actually relevant to use only for aggregated transformations i.e. the "Composite" implementations.
        ///</para>
        ///<para/>
        ///<para>
        ///However, there is also a reasonable behaviour for the "Leaf" implementations
        ///regarding the number of results (always 1) and the "differences" in lat/long for the "different"
        ///implementations i.e. the "difference" should always be zero since there is only one implementation.
        ///</para>
        ///<para/>
        ///<para>
        ///In other words, the method is meaningful only for the "Composite" implementations
        ///but the "Leaf" implementations should not cause exception to be thrown when using
        ///the method but instead logically expected behaviour.
        ///</para>
        ///</summary>
        ///<param name="minimumNumberOfSuccesfulResults">
        ///The minimum number of results for a results to be considered as reliable.
        ///Currently there are three implementations (though one of them can only handle coordinate system used in Sweden)
        ///so you should use a value not larger than 3 (for sweden CRS) or 2 (for non-sweden CRS).
        ///</param>
        ///<param name="maxDeltaValueForXLongitudeAndYLatitude">
        ///The maximum difference in either x/Long or y/Lat to be considered as reliable. 
        ///IMPORTANT note: the unit for the delta value is the unit of the output/result coordinate.
        ///For example if you are using a projected coordinate system with x/Y values in meters then the value 1 (i.e. one meter)
        ///is fairly small, but the value 1 would be very big for "GPS" (WGS84) latitude/longitude values.
        ///</param>
        member this.IsReliable
            (
                minimumNumberOfSuccesfulResults: int,
                maxDeltaValueForXLongitudeAndYLatitude: Double
            ): bool =
            let numberOfResults = this.CrsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults
            let maxX = this.CrsTransformationResultStatistic.MaxDifferenceForXEastingLongitude
            let maxY = this.CrsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude
            let okNumber = numberOfResults >= minimumNumberOfSuccesfulResults
            let okX = maxX <= maxDeltaValueForXLongitudeAndYLatitude
            let okY = maxY <= maxDeltaValueForXLongitudeAndYLatitude
            okNumber && okX && okY

        ///<summary>
        ///This method is not intended for public use from client code.
        ///It is "internal" but but to make it available from the test project, the following 
        ///have been added to the project file:
        ///<code>
        ///<![CDATA[
        ///  <ItemGroup>
        ///    <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
        ///      <_Parameter1>Programmerare.CrsTransformations.Test</_Parameter1>
        ///    </AssemblyAttribute>
        ///  </ItemGroup>
        ///]]>
        ///</code>
        ///</summary>
        static member internal _CreateCrsTransformationResult
            (
                inputCoordinate: CrsCoordinate,
                outputCoordinate: CrsCoordinate,
                exceptionOrNull: Exception,
                isSuccess: Boolean,
                crsTransformationAdapterResultSource: ICrsTransformationAdapter,
                nullableCrsTransformationResultStatistic: CrsTransformationResultStatistic// = null
            ): CrsTransformationResult =
            let mutable exc: Exception = exceptionOrNull
            if( not(isSuccess) && (isNull exc)) then 
                // problem , maybe some items has an exception, and if so then reuse some found exception
                // as the outermost exception in the composite too:
                let allResults = nullableCrsTransformationResultStatistic.GetAllCrsTransformationResults()
                for res in allResults do
                    if not(res.IsSuccess) && not(isNull res.Exception) then 
                        exc <- res.Exception
                // One alternative to simply (as above) reuse 
                // the last exception can be to instead implement code similar as below 
                // from the Kotlin project's implementation:
                //if(this.transformationResultChildren == null || this.transformationResultChildren.size == 0) return exception
                //val sb = StringBuilder()
                //for (transformationResultChild in this.transformationResultChildren) {
                //    if(transformationResultChild.exception != null) {
                //        sb.appendln(transformationResultChild.exception.message)
                //    }
                //}
                //if(sb.isEmpty()){
                //    return null    
                //}
                //else {
                //    sb.appendln("If you want more details with stacktrace you can try iterating the children for exceptions.")
                //    sb.appendln("This composite exception message only contains the 'getMessage' part for each child exception.")
                //    return RuntimeException(sb.toString())
                //}
            CrsTransformationResult(
                inputCoordinate,
                outputCoordinate,
                exc,
                isSuccess,
                crsTransformationAdapterResultSource,
                nullableCrsTransformationResultStatistic
            )
    end // end of the class CrsTransformationResult

// NOTE THAT THERE ARE MORE THAN ONE TYPES IN THIS FILE
// (two types above and one below)
/// --------------------------------------------------------
/// --------------------------------------------------------
/// --------------------------------------------------------
/// Type CrsTransformationResultStatistic:
/// (the THIRD of the three types in this file)

///<summary>
///<para>
///Class providing conveniently available aggregated information from multiple results.
///</para>
///</summary>
and CrsTransformationResultStatistic private
    (
        results: IList<CrsTransformationResult>
    ) =

    let getMaxDiff(values: List<double>) = 
        if (values.Count < 2) then
            0.0
        else 
            let ma = values.Max()
            let mi = values.Min()
            let diff = Math.Abs(ma - mi)
            diff

    // F# Lazy loading: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/lazy-computations

    let _successfulCoordinatesLazyLoaded: Lazy<List<CrsCoordinate>> =
        lazy (
            results.Where(fun r -> r.IsSuccess).Select(fun r -> r.OutputCoordinate).ToList();
        )
        
    let _longitudesLazyLoaded: Lazy<List<double>> = 
        lazy (
            _successfulCoordinatesLazyLoaded.Force().Select(fun c -> c.Longitude).ToList()
        )

    let _latitudesLazyLoaded: Lazy<List<double>> = 
        lazy (
            _successfulCoordinatesLazyLoaded.Force().Select(fun c -> c.Latitude).ToList()
        )

    let _maxDiffLatitudesLazyLoaded: Lazy<double> =
        lazy (
            getMaxDiff(_latitudesLazyLoaded.Force())
        )

    let _maxDiffLongitudesLazyLoaded: Lazy<double> =
        lazy (
            getMaxDiff(_longitudesLazyLoaded.Force())
        )

    let _coordinateMedianLazyLoaded =
        lazy (
            let coords = _successfulCoordinatesLazyLoaded.Force()
            if(coords.Count < 1) then
                invalidOp "No successful result and therefore no median available"
            let medianLat = MedianValueUtility.GetMedianValue(_latitudesLazyLoaded.Force())
            let medianLon = MedianValueUtility.GetMedianValue(_longitudesLazyLoaded.Force())
            let crs = coords.[0].CrsIdentifier // all should have the same CRS which is here assumed to work properly i.e. not validated here
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude
                (
                    medianLon,
                    medianLat,
                    crs
                )
        )

    let _coordinateAverageLazyLoaded =
        lazy (
            let coords = _successfulCoordinatesLazyLoaded.Force()
            if(coords.Count < 1) then
                invalidOp "No successful result and therefore no average available"
            let avgLat = _latitudesLazyLoaded.Force().Average()
            let avgLon = _longitudesLazyLoaded.Force().Average()
            let crs = coords.[0].CrsIdentifier // all should have the same CRS which is here assumed to work properly i.e. not validated here
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude
                (
                    avgLon,
                    avgLat,
                    crs
                )
        )

    let throwExceptionIfPreconditionViolated(isStatisticsAvailable): unit =
        if(not isStatisticsAvailable) then
            invalidOp "Precondition violated. No statistics available"

    // Above: private methods/properties
    // ----------------------------------------------------------
    // Below: public methods/properties

    ///<returns>
    ///list of all transformation results
    ///</returns>
    member this.GetAllCrsTransformationResults(): IList<CrsTransformationResult> = results

    ///<value>
    ///true if there is at least one succesful result but otherwise false.
    ///</value>
    member this.IsStatisticsAvailable = this.NumberOfPotentiallySuccesfulResults > 0

    ///<value>
    ///The number of potentially succesful results.
    ///The semantic reason for using "potentially" is that there is 
    ///no guarantee that a result is correct since an implementation
    ///might choose to return some calculated coordinates 
    ///even though the target (or source) CRS is not intended 
    ///for the area. However, this property returns the number 
    ///of results with *NO OBVIOUS* problems e.g. thrown exception.
    ///</value>
    member this.NumberOfPotentiallySuccesfulResults = _successfulCoordinatesLazyLoaded.Force().Count

    ///<value>
    ///a coordinate with the average X/Longitude and the average Y/Latitude
    ///Precondition: IsStatisticsAvailable must return true
    ///</value>
    member this.CoordinateAverage =
        throwExceptionIfPreconditionViolated(this.IsStatisticsAvailable)
        _coordinateAverageLazyLoaded.Force()
    
    ///<value>
    ///a coordinate with the median X/Longitude and the median Y/Latitude
    ///Precondition: isStatisticsAvailable must return true
    ///</value>
    member this.CoordinateMedian =
        throwExceptionIfPreconditionViolated(this.IsStatisticsAvailable)
        _coordinateMedianLazyLoaded.Force()

    ///<value>
    ///The maximal difference in Y/Latitude values
    ///between the coordinate with the smallest and the largest Y/Latitude values.
    ///</value>
    member this.MaxDifferenceForYNorthingLatitude =
        throwExceptionIfPreconditionViolated(this.IsStatisticsAvailable)
        _maxDiffLatitudesLazyLoaded.Force() // F# Lazy loading: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/lazy-computations
    
    ///<value>
    ///The maximal difference in X/Longitude values
    ///between the coordinate with the smallest and the largest X/Longitude values.
    ///</value>
    member this.MaxDifferenceForXEastingLongitude =
        throwExceptionIfPreconditionViolated(this.IsStatisticsAvailable)
        _maxDiffLongitudesLazyLoaded.Force() // F# Lazy loading: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/lazy-computations

    ///<summary>
    ///Factory method not intended to be used from client code.
    ///Therefore it is "internal" but still available from test code 
    ///because of the following configuration in the project file:
    ///<![CDATA[
    ///<ItemGroup>
    ///    <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
    ///        <_Parameter1>Programmerare.CrsTransformations.Test</_Parameter1>
    ///    </AssemblyAttribute>
    ///</ItemGroup>
    ///]]>
    ///</summary>
    static member internal _CreateCrsTransformationResultStatistic
        (
            results: IList<CrsTransformationResult>
        ): CrsTransformationResultStatistic =
            CrsTransformationResultStatistic(results)
