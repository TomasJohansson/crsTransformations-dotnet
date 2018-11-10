package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import com.programmerare.crsTransformations.coordinate.CrsCoordinate

internal abstract class CompositeStrategyBase
    (private val crsTransformationAdapters: List<CrsTransformationAdapter>)
    : CompositeStrategy {

    override final fun getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked(): List<CrsTransformationAdapter> {
        return crsTransformationAdapters
    }

    /**
     * This base class method is reusable from both subclasses
     * that calculates the median or average which is provided with the last
     * function parameter of the method
     */
    protected fun calculateAggregatedResultBase(
            allResults: List<CrsTransformationResult>,
            inputCoordinate: CrsCoordinate,
            crsTransformationAdapterThatCreatedTheResult: CrsTransformationAdapter,
            crsTransformationResultStatistic: CrsTransformationResultStatistic,
            medianOrAverage: () -> CrsCoordinate
    ): CrsTransformationResult {
        if(crsTransformationResultStatistic.isStatisticsAvailable()) {
            //  val coordRes = crsTransformationResultStatistic.getCoordinateMedian() // THE ONLY DIFFERENCE in the above mentioned two classes
            //  val coordRes = crsTransformationResultStatistic.getCoordinateAverage()  // THE ONLY DIFFERENCE in the above mentioned two classes
            val coordRes: CrsCoordinate = medianOrAverage() // this line replaced the above two lines in different subclasses when doing refactoring
            return CrsTransformationResult(
                inputCoordinate,
                outputCoordinate = coordRes,
                exception = null,
                isSuccess = true,
                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                transformationResultChildren = allResults,
                _nullableCrsTransformationResultStatistic = crsTransformationResultStatistic
            )
        }
        else {
            return CrsTransformationResult(
                inputCoordinate,
                outputCoordinate = null,
                exception = null,
                isSuccess = false,
                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                transformationResultChildren = allResults,
                _nullableCrsTransformationResultStatistic = crsTransformationResultStatistic
            )
        }
    }

    override fun getAdapteeType() : CrsTransformationAdapteeType {
        return CrsTransformationAdapteeType.UNSPECIFIED_COMPOSITE
    }
}