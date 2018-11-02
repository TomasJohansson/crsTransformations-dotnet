package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.coordinate.Coordinate
import java.lang.IllegalStateException
import java.lang.RuntimeException

class CrsTransformationResultImplementation(
        override val inputCoordinate: Coordinate,
        outputCoordinate: Coordinate?,
        override val exception: Throwable?,
        override val isSuccess: Boolean,
        override val crsTransformationAdapterResultSource: CrsTransformationAdapter,
        override val transformationResultChildren: List<CrsTransformationResult> = listOf<CrsTransformationResult>(), // empty list default for the "leaf" transformations, but the composite should have non-empty list)
        _nullableCrsTransformationResultStatistic: CrsTransformationResultStatistic? = null
): CrsTransformationResult {

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

    private val _crsTransformationResultStatistic: CrsTransformationResultStatistic by lazy {
        if(_nullableCrsTransformationResultStatistic != null) {
            _nullableCrsTransformationResultStatistic
        }
        else if(this.transformationResultChildren.size == 0) {
            CrsTransformationResultStatistic(listOf<CrsTransformationResult>(this))
        }
        else {
            CrsTransformationResultStatistic(transformationResultChildren)
        }
    }

    override val crsTransformationResultStatistic: CrsTransformationResultStatistic
        get() {
            return _crsTransformationResultStatistic
        }

    override fun isReliable(
        minimumNumberOfSuccesfulResults: Int,
        maxDeltaValueForXLongitudeAndYLatitude: Double
    ): Boolean {
        val numberOfResults = this.crsTransformationResultStatistic.getNumberOfResults()
        val maxX = this.crsTransformationResultStatistic.getMaxDiffXLongitude()
        val maxY = this.crsTransformationResultStatistic.getMaxDiffYLatitude()
        val okNumber = numberOfResults >= minimumNumberOfSuccesfulResults
        val okX = maxX <= maxDeltaValueForXLongitudeAndYLatitude
        val okY = maxY <= maxDeltaValueForXLongitudeAndYLatitude
        return okNumber && okX && okY
    }

}