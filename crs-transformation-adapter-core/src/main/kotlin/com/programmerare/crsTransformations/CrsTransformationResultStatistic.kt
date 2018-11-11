package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.coordinate.createFromXEastingLongitudeAndYNorthingLatitude
import com.programmerare.crsTransformations.utils.MedianValueUtility
import java.lang.RuntimeException

/**
 * Class providing conveniently available aggregated information from multiple results. 
 */
class CrsTransformationResultStatistic(
    private val results: List<CrsTransformationResult>
) {

    private val _sucessfulCoordinatesLazyLoaded: List<CrsCoordinate> by lazy {
        results.filter { it.isSuccess }.map { it.outputCoordinate }
    }

    private fun throwExceptionIfPreconditionViolated() {
        if(!isStatisticsAvailable()) {
            throw RuntimeException("Precondition violated. No staticis available")
        }
    }

    private val _longitudesLazyLoaded: List<Double> by lazy {
        _sucessfulCoordinatesLazyLoaded.map { it.xEastingLongitude }
    }
    private val _latitudesLazyLoaded: List<Double> by lazy {
        _sucessfulCoordinatesLazyLoaded.map { it.yNorthingLatitude }
    }

    private fun getMaxDiff(values: List<Double>): Double {
        if(values.size < 2) {
            return 0.0
        }
        else {
            val sortedValues = values.sorted()
            val diff = Math.abs(sortedValues.get(0) - sortedValues.get(sortedValues.size-1))
            return diff
        }
    }

    private val _maxDiffLatitudesLazyLoaded: Double by lazy {
        getMaxDiff(_latitudesLazyLoaded)
    }

    /**
     * @return the maximal difference in Y/Latitude values 
     *      between the coordinate with the smallest and the largest Y/Latitude values.   
     */
    fun getMaxDifferenceForYNorthingLatitude(): Double {
        return _maxDiffLatitudesLazyLoaded
    }

    private val _maxDiffLongitudesLazyLoaded: Double by lazy {
        getMaxDiff(_longitudesLazyLoaded)
    }

    /**
     * @return the maximal difference in X/Longitude values
     *      between the coordinate with the smallest and the largest X/Longitude values.
     */    
    fun getMaxDifferenceForXEastingLongitude(): Double {
        return _maxDiffLongitudesLazyLoaded
    }

    private val _coordinateMedianLazyLoaded: CrsCoordinate by lazy {
        val lon = MedianValueUtility.getMedianValue(_longitudesLazyLoaded)
        val lat = MedianValueUtility.getMedianValue(_latitudesLazyLoaded)
        val coord = createFromXEastingLongitudeAndYNorthingLatitude(lon, lat, _sucessfulCoordinatesLazyLoaded.get(0).crsIdentifier)
        coord
    }
    
    private val _coordinateAverageLazyLoaded: CrsCoordinate by lazy {
        createFromXEastingLongitudeAndYNorthingLatitude(_longitudesLazyLoaded.average(), _latitudesLazyLoaded.average(), _sucessfulCoordinatesLazyLoaded.get(0).crsIdentifier)
    }

    /**
     * @return true if there is at least one succesful result but otherwise false.
     */
    fun isStatisticsAvailable(): Boolean {
        return getNumberOfResults() > 0
    }

    /**
     * @return the number of succesful results
     */    
    fun getNumberOfResults(): Int {
        return _sucessfulCoordinatesLazyLoaded.size
    }


    /**
     * Precondition: isStatisticsAvailable must return true
     * @return a coordinate with the median X/Longitude and the median Y/Latitude 
     */
    fun getCoordinateMedian(): CrsCoordinate {
        throwExceptionIfPreconditionViolated()
        return _coordinateMedianLazyLoaded
    }

    /**
     * Precondition: isStatisticsAvailable must return true
     * @return a coordinate with the average X/Longitude and the average Y/Latitude
     */
    fun getCoordinateAverage(): CrsCoordinate {
        throwExceptionIfPreconditionViolated()
        return _coordinateAverageLazyLoaded
    }
}