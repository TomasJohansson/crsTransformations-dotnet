package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.coordinate.Coordinate
import java.lang.IllegalStateException
import java.lang.RuntimeException

class TransformResultImplementation(
        override val inputCoordinate: Coordinate,
        outputCoordinate: Coordinate?,
        override val exception: Throwable?,
        override val isSuccess: Boolean,
        override val crsTransformationAdapterThatCreatedTheResult: CrsTransformationAdapter,
        override val subResults: List<TransformResult> = listOf<TransformResult>(), // empty list default for the "leaf" transformations, but the composite should have non-empty list)
        _nullableResultsStatistic: ResultsStatistic? = null
): TransformResult {

    private val _outputCoordinate: Coordinate? = outputCoordinate

    init {
        if(isSuccess && outputCoordinate == null) {
            throw IllegalStateException("Unvalid object construction. If success then output coordinate should NOT be null")
        }
        if(!isSuccess && outputCoordinate != null) {
            throw IllegalStateException("Unvalid object construction. If NOT success then output coordinate should be null")
        }
    }

    /**
     * Precondition: The success property must return true
     */
    override val outputCoordinate: Coordinate
        get() {
            if(!isSuccess) throw RuntimeException("Pre-condition violated. Coordinate retrieval only allowed if result was success")
            // if the code executes further than above then
            // we have a success and the below coordinate should never be null
            // since that scenario should be enforced in the init construction
            return _outputCoordinate!!
        }

    private val _resultsStatistic: ResultsStatistic by lazy {
        if(_nullableResultsStatistic != null) {
            _nullableResultsStatistic
        }
        else if(this.subResults.size == 0) {
            ResultsStatistic(listOf<TransformResult>(this))
        }
        else {
            ResultsStatistic(subResults)
        }
    }

    override val resultsStatistic: ResultsStatistic
        get() {
            return _resultsStatistic
        }

    override fun isReliable(
        minimumNumberOfSuccesfulResults: Int,
        maxDeltaValueForXLongitudeAndYLatitude: Double
    ): Boolean {
        val numberOfResults = this.resultsStatistic.getNumberOfResults()
        val maxX = this.resultsStatistic.getMaxDiffXLongitude()
        val maxY = this.resultsStatistic.getMaxDiffYLatitude()
        val okNumber = numberOfResults >= minimumNumberOfSuccesfulResults
        val okX = maxX <= maxDeltaValueForXLongitudeAndYLatitude
        val okY = maxY <= maxDeltaValueForXLongitudeAndYLatitude
        return okNumber && okX && okY
    }

}