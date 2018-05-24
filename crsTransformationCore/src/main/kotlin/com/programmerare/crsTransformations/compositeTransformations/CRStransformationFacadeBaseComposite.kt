package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*

// subclasses:
// CRStransformationFacadeChainOfResponsibility
// CRStransformationFacadeAverage
abstract class CRStransformationFacadeBaseComposite(private val crsTransformationFacades: java.util.List<CRStransformationFacade>) : CRStransformationFacadeBase(), CRStransformationFacade {

    override fun transform(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): Coordinate {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun transformToResultObject(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): TransformResult {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }
}
