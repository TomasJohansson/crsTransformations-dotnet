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

    private val _sucessfulCoordinates: List<CrsCoordinate> by lazy {
        results.filter { it.isSuccess }.map { it.outputCoordinate }
    }

    private fun throwExceptionIfPreconditionViolated() {
        if(!isStatisticsAvailable()) {
            throw RuntimeException("Precondition violated. No staticis available")
        }
    }

    private val _longitudes: List<Double> by lazy {
        _sucessfulCoordinates.map { it.xEastingLongitude }
    }
    private val _latitudes: List<Double> by lazy {
        _sucessfulCoordinates.map { it.yNorthingLatitude }
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

    private val _maxDiffLatitudes: Double by lazy {
        getMaxDiff(_latitudes)
    }

    /**
     * @return the maximal difference in Y/Latitude values 
     *      between the coordinate with the smallest and the largest Y/Latitude values.   
     */
    fun getMaxDifferenceForYNorthingLatitude(): Double {
        return _maxDiffLatitudes
    }

    private val _maxDiffLongitudes: Double by lazy {
        getMaxDiff(_longitudes)
    }

    /**
     * @return the maximal difference in X/Longitude values
     *      between the coordinate with the smallest and the largest X/Longitude values.
     */    
    fun getMaxDifferenceForXEastingLongitude(): Double {
        return _maxDiffLongitudes
    }

    private val _coordinateMedian: CrsCoordinate by lazy {
        val lon = MedianValueUtility.getMedianValue(_longitudes)
        val lat = MedianValueUtility.getMedianValue(_latitudes)
        val coord = createFromXEastingLongitudeAndYNorthingLatitude(lon, lat, _sucessfulCoordinates.get(0).crsIdentifier)
        coord
    }
    private val _coordinateAverage: CrsCoordinate by lazy {
        createFromXEastingLongitudeAndYNorthingLatitude(_longitudes.average(), _latitudes.average(), _sucessfulCoordinates.get(0).crsIdentifier)
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
        return _sucessfulCoordinates.size
    }


    /**
     * Precondition: isStatisticsAvailable must return true
     * @return a coordinate with the median X/Longitude and the median Y/Latitude 
     */
    fun getCoordinateMedian(): CrsCoordinate {
        throwExceptionIfPreconditionViolated()
        return _coordinateMedian
    }

    /**
     * Precondition: isStatisticsAvailable must return true
     * @return a coordinate with the average X/Longitude and the average Y/Latitude
     */
    fun getCoordinateAverage(): CrsCoordinate {
        throwExceptionIfPreconditionViolated()
        return _coordinateAverage
    }
}