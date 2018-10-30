package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.utils.MedianValueUtility
import java.lang.RuntimeException

class ResultsStatistic(private val results: List<TransformResult>) {

    private val _sucessfulCoordinates: List<Coordinate> by lazy {
        results.filter { it.isSuccess }.map { it.outputCoordinate }
    }

    private fun throwExceptionIfPreconditionViolated() {
        if(!isStatisticsAvailable()) {
            throw RuntimeException("Precondition violated. No staticis available")
        }
    }

    private val _longitudes: List<Double> by lazy {
        _sucessfulCoordinates.map { it.xLongitude }
    }
    private val _latitudes: List<Double> by lazy {
        _sucessfulCoordinates.map { it.yLatitude }
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

    fun getMaxDiffLatitude(): Double {
        return _maxDiffLatitudes
    }

    private val _maxDiffLongitudes: Double by lazy {
        getMaxDiff(_longitudes)
    }

    fun getMaxDiffLongitude(): Double {
        return _maxDiffLongitudes
    }

    private val _coordinateMean: Coordinate by lazy {
        val lon = MedianValueUtility.getMedianValue(_longitudes)
        val lat = MedianValueUtility.getMedianValue(_latitudes)
        val coord = Coordinate.createFromXLongitudeYLatitude(lon, lat, _sucessfulCoordinates.get(0).crsIdentifier)
        coord
    }
    private val _coordinateAverage: Coordinate by lazy {
        Coordinate.createFromXLongitudeYLatitude(_longitudes.average(), _latitudes.average(), _sucessfulCoordinates.get(0).crsIdentifier)
    }


    fun isStatisticsAvailable(): Boolean {
        return getNumberOfResults() > 0
    }

    fun getNumberOfResults(): Int {
        return _sucessfulCoordinates.size
    }


    /**
     * Precondition: isStatisticsAvailable must return true
     */
    fun getCoordinateMean(): Coordinate {
        throwExceptionIfPreconditionViolated()
        return _coordinateMean
    }

    /**
     * Precondition: isStatisticsAvailable must return true
     */
    fun getCoordinateAverage(): Coordinate {
        throwExceptionIfPreconditionViolated()
        return _coordinateAverage
    }
}