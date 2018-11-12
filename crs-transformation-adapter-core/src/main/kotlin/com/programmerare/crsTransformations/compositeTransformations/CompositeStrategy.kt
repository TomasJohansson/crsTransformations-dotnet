package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationAdapteeType
import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationResult

/**
 * Kotlin "internal" interface. 
 * The methods are not intended for public use from client code.
 */
internal interface CompositeStrategy {

    /**
     * The method is not intended for public use from client code. 
     */
    fun _getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked(): List<CrsTransformationAdapter>

    /**
     * The method is not intended for public use from client code.
     */    
    fun _shouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious: CrsTransformationResult?) : Boolean

    /**
     * The method is not intended for public use from client code.
     */    
    fun _calculateAggregatedResult(
        allResults: List<CrsTransformationResult>,
        inputCoordinate: CrsCoordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
        crsTransformationAdapterThatCreatedTheResult: CrsTransformationAdapter
    ): CrsTransformationResult

    /**
     * The method is not intended for public use from client code.
     */    
    fun _getAdapteeType() : CrsTransformationAdapteeType
}