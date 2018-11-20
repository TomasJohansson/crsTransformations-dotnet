namespace com.programmerare.crsTransformations

open System.Collections.Generic

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
type CrsTransformationResultStatistic // TODO maybe private constructor(
    (
        results: IList<CrsTransformationResult>
    ) =

    let throwExceptionIfPreconditionViolated(): unit = ()
        //if (!isStatisticsAvailable()) {
        //    throw RuntimeException("Precondition violated. No statistics available")

    
    let getMaxDiff(values: List<double>) = 1.0//: Double {
        //if (values.size < 2) {
        //    return 0.0
        //} else {
        //    val sortedValues = values.sorted()
        //    val diff = Math.abs(sortedValues.get(0) - sortedValues.get(sortedValues.size - 1))
        //    return diff
    
    let _sucessfulCoordinatesLazyLoaded = null//: List<CrsCoordinate> by lazy {
        //results.filter { it.isSuccess }.map { it.outputCoordinate }

    let _longitudesLazyLoaded = null// List<Double> by lazy {
        //_sucessfulCoordinatesLazyLoaded.map { it.xEastingLongitude }

    let _latitudesLazyLoaded = null//: List<Double> by lazy {
        //_sucessfulCoordinatesLazyLoaded.map { it.yNorthingLatitude }

    let _maxDiffLatitudesLazyLoaded = null//: Double by lazy {
        //getMaxDiff(_latitudesLazyLoaded)
    

    let _maxDiffLongitudesLazyLoaded = null//: Double by lazy {
        //getMaxDiff(_longitudesLazyLoaded)

    let _coordinateMedianLazyLoaded = null//: CrsCoordinate by lazy {
        //val lon = MedianValueUtility.getMedianValue(_longitudesLazyLoaded)
        //val lat = MedianValueUtility.getMedianValue(_latitudesLazyLoaded)
        //val coord = createFromXEastingLongitudeAndYNorthingLatitude(lon, lat, _sucessfulCoordinatesLazyLoaded.get(0).crsIdentifier)
        //coord

    let _coordinateAverageLazyLoaded = null//: CrsCoordinate by lazy {
        //createFromXEastingLongitudeAndYNorthingLatitude(_longitudesLazyLoaded.average(), _latitudesLazyLoaded.average(), _sucessfulCoordinatesLazyLoaded.get(0).crsIdentifier)

    // Above: private methods/properties
    // ----------------------------------------------------------
    // Below: public methods

    (*
     * @return true if there is at least one succesful result but otherwise false.
     *)
    member this.IsStatisticsAvailable = false// TODO getNumberOfResults() > 0

    (*
     * @return the number of succesful results
     *)
    member this.NumberOfResults = 0 // TODO  _sucessfulCoordinatesLazyLoaded.size

    (*
     * Precondition: isStatisticsAvailable must return true
     * @return a coordinate with the average X/Longitude and the average Y/Latitude
     *)
    member this.CoordinateAverage = null;// TODO CrsCoordinate {
        //throwExceptionIfPreconditionViolated()
        //return _coordinateAverageLazyLoaded
    

    (*
     * Precondition: isStatisticsAvailable must return true
     * @return a coordinate with the median X/Longitude and the median Y/Latitude
     *)
    member this.CoordinateMedian = null// CrsCoordinate {
        //throwExceptionIfPreconditionViolated()
        //return _coordinateMedianLazyLoaded

    (*
     * @return the maximal difference in Y/Latitude values
     *      between the coordinate with the smallest and the largest Y/Latitude values.
     *)
    member this.MaxDifferenceForYNorthingLatitude = null // Double {
        //return _maxDiffLatitudesLazyLoaded
    
    
    (*
     * @return the maximal difference in X/Longitude values
     *      between the coordinate with the smallest and the largest X/Longitude values.
     *)
    member this.MaxDifferenceForXEastingLongitude = null //  Double {
        //return _maxDiffLongitudesLazyLoaded

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

