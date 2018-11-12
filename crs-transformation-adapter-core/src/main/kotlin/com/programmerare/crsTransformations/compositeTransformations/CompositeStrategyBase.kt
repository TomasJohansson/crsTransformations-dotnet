package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import java.lang.RuntimeException

internal abstract class CompositeStrategyBase protected constructor
    (private val crsTransformationAdapters: List<CrsTransformationAdapter>)
    : CompositeStrategy {

    init {
        if(crsTransformationAdapters == null || crsTransformationAdapters.size < 1) {
            throw RuntimeException("'Composite' adapter can not be created with an empty list of 'leaf' adapters") 
        }
    }
    
    override final fun _getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked(): List<CrsTransformationAdapter> {
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
            medianOrAverage: (CrsTransformationResultStatistic) -> CrsCoordinate
    ): CrsTransformationResult {
        if(crsTransformationResultStatistic.isStatisticsAvailable()) {
            //  val coordRes = crsTransformationResultStatistic.getCoordinateMedian() // THE ONLY DIFFERENCE in the above mentioned two classes
            //  val coordRes = crsTransformationResultStatistic.getCoordinateAverage()  // THE ONLY DIFFERENCE in the above mentioned two classes
            val coordRes: CrsCoordinate = medianOrAverage(crsTransformationResultStatistic) // this line replaced the above two lines in different subclasses when doing refactoring
            return CrsTransformationResult._createCrsTransformationResult(
                inputCoordinate,
                outputCoordinate = coordRes,
                exception = null,
                isSuccess = true,
                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                transformationResultChildren = allResults,
                nullableCrsTransformationResultStatistic = crsTransformationResultStatistic
            )
        }
        else {
            return CrsTransformationResult._createCrsTransformationResult(
                inputCoordinate,
                outputCoordinate = null,
                exception = null,
                isSuccess = false,
                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                transformationResultChildren = allResults,
                nullableCrsTransformationResultStatistic = crsTransformationResultStatistic
            )
        }
    }

    override fun _getAdapteeType() : CrsTransformationAdapteeType {
        return CrsTransformationAdapteeType.UNSPECIFIED_COMPOSITE
    }
}