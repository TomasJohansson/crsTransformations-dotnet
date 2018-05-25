package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import java.lang.RuntimeException

// subclasses:
// CrsTransformationFacadeChainOfResponsibility
// CrsTransformationFacadeAverage
abstract class CrsTransformationFacadeBaseComposite(protected val crsTransformationFacades: java.util.List<CrsTransformationFacade>) : CrsTransformationFacadeBase(), CrsTransformationFacade {

    override final fun transform(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): Coordinate {
        val transformResult = transformToResultObject(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
        if(transformResult.isSuccess) {
            return transformResult.outputCoordinate
        }
        else {
            throw RuntimeException("Transformation failed")
        }
    }
}
