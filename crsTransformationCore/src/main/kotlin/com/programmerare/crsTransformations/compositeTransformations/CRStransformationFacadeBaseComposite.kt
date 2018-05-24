package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CRStransformationFacade
import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsIdentifier
import com.programmerare.crsTransformations.TransformResult

// subclasses:
// CRStransformationFacadeChainOfResponsibility
// CRStransformationFacadeAverage
abstract class CRStransformationFacadeBaseComposite(private val crsTransformationFacades: java.util.List<CRStransformationFacade>) : CRStransformationFacade {
    override fun transform(inputCoordinate: Coordinate, epsgNumberForOutputCoordinateSystem: Int): Coordinate {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun transform(inputCoordinate: Coordinate, crsCodeForOutputCoordinateSystem: String): Coordinate {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun transform(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): Coordinate {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun transformToResultObject(inputCoordinate: Coordinate, epsgNumberForOutputCoordinateSystem: Int): TransformResult {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun transformToResultObject(inputCoordinate: Coordinate, crsCodeForOutputCoordinateSystem: String): TransformResult {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun transformToResultObject(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): TransformResult {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }
}
