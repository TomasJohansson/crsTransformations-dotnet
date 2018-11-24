namespace Programmerare.CrsTransformations

open System
open System.Linq
open System.Collections.Generic
open Programmerare.CrsTransformations.Coordinate
open Programmerare.CrsTransformations.Identifier

// Note 1 : There are three types in this file separated with "and" 
//          (or actually the keyword "and" is used instead of "type" for the seoncd and third types in this file)
//          The reason is that F# compiles files in a certain order 
//          which often is not a problem since you often want to avoid bidirectional dependencies
//          but in this case I think it is reasonable/natural with som mutual 
//          dependencies between the three types in this file 
//          which is the reason for putting them in the same file 
//          and solve the problem by doing that and by using the keyword "and".
// Note 2 : The comments above methods are still written for the Kotlin/JVM/Java version
// Note 3 : Some Kotlin code still remains in the file (within comments) 
//          which eventually should become but currently works as a reminder 
//          about how to continue with the development of the .NET version
//          instead of mentally reinventing the wheel e.g. 
//          thinking about appropriate names of methods/properties...

(*
 * This adapter interface is the core type of this CRS transtiomation library.
 * 
 * It defines six transform methods.
 * 
 * Three of them will only return a coordinate with the result,
 * while the other three will return a result object
 * which also contains more information e.g. all the individual 'leaf'
 * results if the implementing class was a 'composite'.  
 *
 * The difference between the three methods (returning the same type)
 * is just the last parameter which is either an integer value
 * with an EPSG code (e.g. 4326) or a string (an integer value but also with a "EPSG:"-prefix e.g. "EPSG:4326")
 * or an instance of CrsIdentifier.  
 *
 * If a transform method without the CrsIdentifier parameter is used then
 * the CrsIdentifier will be created by the other transform methods.
 * 
 * In other words, the methods with integer or string parameter are
 * convenience methods. If you are going to do many transformations
 * you may want to create the CrsIdentifier object once yourself
 * and then use a method using it as a parameter.  
 *
 * The methods 'transformToCoordinate' can throw exception when the transformation fails.
 * 
 * The methods 'transform' should always return a result object rather than throwing an exception.
 * 
 * If you use a method returning the result object then you should
 * check for failures with 'TransformationResult.isSuccess'.
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 *)
[<AllowNullLiteral>] // C# interop
type ICrsTransformationAdapter =
    interface
        // -------------------------------------------------
        // The three methods returning a coordinate object:

        (*
         * Transforms a coordinate to a coordinate
         * in another coordinate reference system if possible
         * but may throw an exception if the transformation fails.
         *)
        abstract member TransformToCoordinate : CrsCoordinate * int -> CrsCoordinate

        (*
         * Transforms a coordinate to a coordinate
         * in another coordinate reference system if possible
         * but may throw an exception if the transformation fails.
         *)
        abstract member TransformToCoordinate : CrsCoordinate * string -> CrsCoordinate

        (*
         * Transforms a coordinate to a coordinate
         * in another coordinate reference system if possible
         * but may throw an exception if the transformation fails.
         *)
        abstract member TransformToCoordinate : CrsCoordinate * CrsIdentifier -> CrsCoordinate
        // -------------------------------------------------

        // -------------------------------------------------
        // The three methods returning a transformation result object:

        (*
         * Transforms a coordinate to another coordinate reference system.
         * 
         * The method should never throw an exception but instead one of the methods
         * in the result object should be used to check for failure.
         *)
        abstract member Transform : CrsCoordinate * int -> CrsTransformationResult

        (*
         * Transforms a coordinate to another coordinate reference system.
         * 
         * The method should never throw an exception but instead one of the methods
         * in the result object should be used to check for failure.
         *)
        abstract member Transform : CrsCoordinate * string -> CrsTransformationResult

        (*
         * Transforms a coordinate to another coordinate reference system.
         * 
         * The method should never throw an exception but instead one of the methods
         * in the result object should be used to check for failure.
         *)
        abstract member Transform : CrsCoordinate * CrsIdentifier -> CrsTransformationResult
        // -------------------------------------------------

        (*
         * Should normally simply return the full class name (including the package name), 
         * but when implementing test doubles (e.g. Mockito stub)
         * then the method should be implemented by defining different names
         * to simulate that different classes (implementations)
         * should have different weights.
         *)
        abstract member LongNameOfImplementation : string

        (*
         * Should return the unique suffix part of the class name
         * i.e. the class name without the prefix which is common
         * for all implementations.
         *)
        abstract member ShortNameOfImplementation : string

        (*
         * @see CrsTransformationAdapteeType
         *)
        abstract member AdapteeType : CrsTransformationAdapteeType

        (*
         * @return  true if the implementation is a 'composite'
         *          but false if it is a 'leaf' implementation
         *)
        abstract member IsComposite : bool

        (*
         * @return  a list of children/leafs when the implementation
         *          is a 'composite'but if the implementation is a 'leaf'
         *          then an empty list should be returned.
         *)
        abstract member GetTransformationAdapterChildren : unit -> IList<ICrsTransformationAdapter>

    end

// NOTE THAT THERE ARE MORE THAN ONE TYPES IN THIS FILE
// one type above and two below)

// ---------------------------------------
// ---------------------------------------
// ---------------------------------------
// ---------------------------------------
// ---------------------------------------

// NOTE THAT THERE ARE MORE THAN ONE TYPES IN THIS FILE
// one type above and two below)

(*
 * This class is used as result type from the transform method of the adapter interface.
 * @see CrsTransformationAdapter
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 *)

// Normally you define a class with "type" as below:
//type CrsTransformationResult // private
// However, to make it possible for types to refer each other
// one current solution is to put them in the same file 
// and then use "and" as below instead of "type"
and CrsTransformationResult // TODO maybe make a private constructor
    (
        inputCoordinate: CrsCoordinate,
        outputCoordinate: CrsCoordinate, //[<Optional; DefaultParameterValue(null:CrsCoordinate)>] outputCoordinate: CrsCoordinate,
        exceptionOrNull: Exception,
        isSuccess: bool,
        
        crsTransformationAdapterResultSource: ICrsTransformationAdapter,
        // Note that the below parameter is a mutual dependency
        // since CrsTransformationAdapter also uses the CrsTransformationResult
        // as result type for Transform methods
        crsTransformationResultStatistic: CrsTransformationResultStatistic
    ) =
    class
        member this.InputCoordinate = inputCoordinate
        member this.OutputCoordinate = outputCoordinate
        member this.Exception = exceptionOrNull
        member this.IsSuccess = isSuccess
        member this.CrsTransformationAdapterResultSource = crsTransformationAdapterResultSource

        member this.CrsTransformationResultStatistic = 
            if(this.CrsTransformationAdapterResultSource.IsComposite) then
                crsTransformationResultStatistic
            else
                let list = new List<CrsTransformationResult>()
                list.Add(this)
                CrsTransformationResultStatistic(list)
        // TODO refactor the above and the below method !
        // For a leaf the statistics object should use a list with the one and only result
        // but for composites it should all results 
        // but for a leaf the list below should be an empty list
        member this.GetTransformationResultChildren() = 
            if(this.CrsTransformationAdapterResultSource.IsComposite) then
                crsTransformationResultStatistic.GetAllCrsTransformationResults()
            else
                new List<CrsTransformationResult>() :> IList<CrsTransformationResult>

        (*
        * Convenience method intended for "Composite" implementations
        * to easy check that more than one implementation (the specified min number)
        * resulted in the same coordinate (within the specified delta value).
        *
        * If false is returned then you can retrieve the CrsTransformationResultStatistic object
        * to find the details regarding the differences.
        *
        * The method is actually relevant to use only for aggregated transformations i.e. the "Composite" implementations.
        *
        * However, there is also a reasonable behaviour for the "Leaf" implementations
        * regarding the number of results (always 1) and the "differences" in lat/long for the "different"
        * implementations i.e. the "difference" should always be zero since there is only one implementation.
        *
        * In other words, the method is meaningful only for the "Composite" implementations
        * but the "Leaf" implementations should not cause exception to be thrown when using
        * the method but instead logically expected behaviour.
        * @param minimumNumberOfSuccesfulResults specifies the minimum number of results for a results to be considered as reliable.
        *      Currently there are five implementations (though one of them can only handle coordinate system used in Sweden)
        *      so you will probably not want to use a value smaller than 4.
        * @param maxDeltaValueForXLongitudeAndYLatitude specifies the maximum difference in either x/Long or y/Lat to be considered as reliable.
        *      IMPORTANT note: the unit for the delta value is the unit of the output/result coordinate.
        *      For example if you are using a projected coordinate system with x/Y values in meters then the value 1 (i.e. one meter)
        *      is fairly small, but the value 1 would be very big for "GPS" (WGS84) latitude/longitude values.
        *)
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

//    (*
//     * The input coordinate used in the transform that return the result object.
//     *)        
//    val inputCoordinate: CrsCoordinate,

//    (*
//     * @see outputCoordinate
//     *)    
//    private val _outputCoordinate: CrsCoordinate?,

//    (*
//     * Either null or an exception depending on whether or not
//     * the transform resulted in an exception being thrown.
//     *)    
//    _exception: Throwable?,

//    (*
//     * True if the transform was successful or false if it failed.
//     * Note that "successful" does not necessarily mean that the
//     * result is correct but an exception was not thrown
//     * and the result was not "NaN" (Not a Number).
//     *)
//    val isSuccess: Boolean,

//    (*
//     * @return CrsTransformationAdapter the adapter which created the result.
//     *
//     *  It may be useful when a composite adapter is returning a result aggregating many results
//     *  and you want to figure out which result originated from which leaf adapter implementation.
//     *)    
//    val crsTransformationAdapterResultSource: CrsTransformationAdapter,


//    (*
//     * Empty list if the transform implementation is a concrete "Leaf"
//     * implementation, but if it is a composite/aggregating implementation
//     * then all the individual "leaf" results are returned in this list.
//     *)
//    val transformationResultChildren: List<CrsTransformationResult> = listOf<CrsTransformationResult>(), // empty list default for the "leaf" transformations, but the composite should have non-empty list)


//    (*
//     * @see crsTransformationResultStatistic
//     *)    
//    private val _nullableCrsTransformationResultStatistic: CrsTransformationResultStatistic? = null
//) {

//    (*
//     * Either null or an exception depending on whether or not
//     * the transform resulted in an exception being thrown.
//     *)
//    val exception: Throwable?
    
//    init {
//        if(isSuccess && _outputCoordinate == null) {
//            throw IllegalStateException("Unvalid object construction. If success then output coordinate should NOT be null")
//        }
//        if(!isSuccess && _outputCoordinate != null) {
//            throw IllegalStateException("Unvalid object construction. If NOT success then output coordinate should be null")
//        }
        
//        if(_exception != null) { // if exception then should NOT be success and not any resulting coordinate !
//            if(isSuccess || _outputCoordinate != null) {
//                throw IllegalStateException("Unvalid object construction. If exception then output coordinate should be null and success should be false")
//            }
//        }
//        this.exception = getExceptionIfNotNullButOtherwiseTryToGetExceptionsFromChildrenExceptionsIfExisting(_exception)
        
        
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
    
//    private fun getExceptionIfNotNullButOtherwiseTryToGetExceptionsFromChildrenExceptionsIfExisting(exception: Throwable?): Throwable? {
//        if(exception != null) return exception
//        if(this.transformationResultChildren == null || this.transformationResultChildren.size == 0) return exception
//        val sb = StringBuilder()
//        for (transformationResultChild in this.transformationResultChildren) {
//            if(transformationResultChild.exception != null) {
//                sb.appendln(transformationResultChild.exception.message)
//            }
//        }
//        if(sb.isEmpty()){
//            return null    
//        }
//        else {
//            sb.appendln("If you want more details with stacktrace you can try iterating the children for exceptions.")
//            sb.appendln("This composite exception message only contains the 'getMessage' part for each child exception.")
//            return RuntimeException(sb.toString())
//        }
//    }

//    (*
//     * The coordinate which is the result from the transform.
//     * 
//     * Precondition: Verify that the success property return true before using this accessor.
//     *  If it returns false, then an exception will be thrown.
//     *
//     * Depending on the adapter implementation, the output coordinate
//     * can either be a direct result from one specific 'leaf' adaptee implementation
//     * or it can be an aggregated result (i.e. median or average) from
//     * a 'composite' implementation.
//     *)
//    val outputCoordinate: CrsCoordinate
//        get() {
//            if(!isSuccess) throw RuntimeException("Pre-condition violated. Coordinate retrieval only allowed if result was success")
//            // if the code executes further than above then
//            // we have a success and the below coordinate should never be null
//            // since that scenario should be enforced in the init construction
//            return _outputCoordinate!!
//        }

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

//    (*
//     * An object with conveniently available aggregating information about the
//     * results for the different implementations, which is useful for composite implementations.
//     * For a leaf implementation this method is not meaningful.
//     * It is a convenience method in the sense that the information provided
//     * can be calculated from client code by iterating the leafs/children of a composite.
//     * @CrsTransformationResultStatistic
//     *)
//    val crsTransformationResultStatistic: CrsTransformationResultStatistic
//        get() {
//            return _crsTransformationResultStatisticLazyLoaded
//        }

//        (*
//         * This method is not intended for public use from client code.
//         *)
        static member _CreateCrsTransformationResult
            (
                inputCoordinate: CrsCoordinate,
                outputCoordinate: CrsCoordinate,
                exceptionOrNull: Exception,
                isSuccess: Boolean,
                crsTransformationAdapterResultSource: ICrsTransformationAdapter,
                nullableCrsTransformationResultStatistic: CrsTransformationResultStatistic// = null
            ): CrsTransformationResult =
            CrsTransformationResult(
                inputCoordinate,
                outputCoordinate,
                exceptionOrNull,
                isSuccess,
                crsTransformationAdapterResultSource,
                nullableCrsTransformationResultStatistic
            )
    end

// NOTE THAT THERE ARE MORE THAN ONE TYPES IN THIS FILE
// two types above and one below)

// ---------------------------------------
// ---------------------------------------
// ---------------------------------------
// ---------------------------------------
// ---------------------------------------

// NOTE THAT THERE ARE MORE THAN ONE TYPES IN THIS FILE
// two types above and one below)

// TODO: the class CrsTransformationResultStatistic is not implemented yet

(*
 * Class providing conveniently available aggregated information from multiple results.
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 *)
and CrsTransformationResultStatistic // TODO maybe private constructor(
    (
        results: IList<CrsTransformationResult>
    ) =

    let getMedianValue(values: List<double>) = 
        //val lon = MedianValueUtility.getMedianValue(_longitudesLazyLoaded)
        //val lat = MedianValueUtility.getMedianValue(_latitudesLazyLoaded)
        //val coord = createFromXEastingLongitudeAndYNorthingLatitude(lon, lat, _sucessfulCoordinatesLazyLoaded.get(0).crsIdentifier)
        //coord
        // TODO: improve this median currently O(n log n) instead of O(n)
        // https://stackoverflow.com/questions/4140719/calculate-median-in-c-sharp
        // https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/Statistics/ArrayStatistics.cs
        // the current below implementation is based on the Kotlin 
        // implemenation Programmerare.CrsTransformations.Core\crsTransformations\utils\MedianValueUtility.kt
        values.Sort()
        let middle = values.Count / 2
        if (values.Count % 2 = 1) then
            values.[middle]
        else
            (values.[middle-1] + values.[middle]) / 2.0
        

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

    let _coordinateMedianLazyLoaded  =
        lazy (
            let medianLat = getMedianValue(_latitudesLazyLoaded.Force())
            let medianLon = getMedianValue(_longitudesLazyLoaded.Force())
            let coords = _successfulCoordinatesLazyLoaded.Force()
            if(coords.Count < 1) then
                invalidOp "No successful result and therefore no average available"
            let crs = coords.[0].CrsIdentifier
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude
                (
                    medianLon,
                    medianLat,
                    crs
                )
        )

    let _coordinateAverageLazyLoaded =
        lazy (
            let avgLat = _latitudesLazyLoaded.Force().Average()
            let avgLon = _longitudesLazyLoaded.Force().Average()
            let coords = _successfulCoordinatesLazyLoaded.Force()
            if(coords.Count < 1) then
                invalidOp "No successful result and therefore no average available"
            let crs = coords.[0].CrsIdentifier
            // All CRS should have the same CRS
            // Theoretically they might have different which would be a bug 
            // so maybe should iterate them to check and throw exception if different ...
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
    // Below: public methods

    member this.GetAllCrsTransformationResults(): IList<CrsTransformationResult> = results
    (*
     * @return true if there is at least one succesful result but otherwise false.
     *)
    member this.IsStatisticsAvailable = this.NumberOfPotentiallySuccesfulResults > 0

    (*
     * the number of potentially succesful results.
     * The reason for "potentially" is that there is 
     * no guarantee that a result is correct since an implementation
     * might choose to return some calculated coordinates 
     * even though the target (or source) CRS is not intended 
     * for the area. However, this property returns the number 
     * of result with *NO OBVIOUS* problems e.g. thrown exception.
     *)
    member this.NumberOfPotentiallySuccesfulResults = _successfulCoordinatesLazyLoaded.Force().Count

    (*
     * Precondition: isStatisticsAvailable must return true
     * @return a coordinate with the average X/Longitude and the average Y/Latitude
     *)
    member this.CoordinateAverage =
        throwExceptionIfPreconditionViolated(this.IsStatisticsAvailable)
        _coordinateAverageLazyLoaded.Force()
    

    (*
     * Precondition: isStatisticsAvailable must return true
     * @return a coordinate with the median X/Longitude and the median Y/Latitude
     *)
    member this.CoordinateMedian =
        throwExceptionIfPreconditionViolated(this.IsStatisticsAvailable)
        _coordinateMedianLazyLoaded.Force()

    (*
     * @return the maximal difference in Y/Latitude values
     *      between the coordinate with the smallest and the largest Y/Latitude values.
     *)
    member this.MaxDifferenceForYNorthingLatitude =
        throwExceptionIfPreconditionViolated(this.IsStatisticsAvailable)
        _maxDiffLatitudesLazyLoaded.Force() // F# Lazy loading: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/lazy-computations
    
    
    (*
     * @return the maximal difference in X/Longitude values
     *      between the coordinate with the smallest and the largest X/Longitude values.
     *)
    member this.MaxDifferenceForXEastingLongitude =
        throwExceptionIfPreconditionViolated(this.IsStatisticsAvailable)
        _maxDiffLongitudesLazyLoaded.Force() // F# Lazy loading: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/lazy-computations



        // Kotlin:
    //internal companion object {
    //    (*
    //     * This method is not intended for public use from client code.
    //     *)
    //    @JvmStatic
    //    fun _createCrsTransformationResultStatistic(
    //        results: List<CrsTransformationResult>
    //    ): CrsTransformationResultStatistic {
    //        return CrsTransformationResultStatistic(results)
