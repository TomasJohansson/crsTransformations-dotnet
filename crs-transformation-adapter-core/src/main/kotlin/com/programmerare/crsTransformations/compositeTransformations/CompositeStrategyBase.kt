package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*

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
            allResults: List<TransformResult>,
            inputCoordinate: Coordinate,
            crsTransformationAdapterThatCreatedTheResult: CrsTransformationAdapter,
            resultsStatistic: ResultsStatistic,
            medianOrAverage: () -> Coordinate
    ): TransformResult {
        if(resultsStatistic.isStatisticsAvailable()) {
            //  val coordRes = resultsStatistic.getCoordinateMedian() // THE ONLY DIFFERENCE in the above mentioned two classes
            //  val coordRes = resultsStatistic.getCoordinateAverage()  // THE ONLY DIFFERENCE in the above mentioned two classes
            val coordRes: Coordinate = medianOrAverage() // this line replaced the above two lines in different subclasses when doing refactoring
            return TransformResultImplementation(
                inputCoordinate,
                outputCoordinate = coordRes,
                exception = null,
                isSuccess = true,
                crsTransformationAdapterThatCreatedTheResult = crsTransformationAdapterThatCreatedTheResult,
                subResults = allResults,
                _nullableResultsStatistic = resultsStatistic
            )
        }
        else {
            return TransformResultImplementation(
                inputCoordinate,
                outputCoordinate = null,
                exception = null,
                isSuccess = false,
                crsTransformationAdapterThatCreatedTheResult = crsTransformationAdapterThatCreatedTheResult,
                subResults = allResults,
                _nullableResultsStatistic = resultsStatistic
            )
        }
    }

}