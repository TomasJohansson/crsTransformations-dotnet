package com.programmerare.crsTransformations

abstract class CRStransformationFacadeBase : CRStransformationFacade {

//    override fun transform(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): Coordinate {
//        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
//    }

    override final fun transform(inputCoordinate: Coordinate, crsCodeForOutputCoordinateSystem: String): Coordinate {
        return transform(inputCoordinate, CrsIdentifier.createFromCrsCode(crsCodeForOutputCoordinateSystem))
    }

    override final fun transform(inputCoordinate: Coordinate, epsgNumberForOutputCoordinateSystem: Int): Coordinate {
        return transform(inputCoordinate, CrsIdentifier.createFromEpsgNumber(epsgNumberForOutputCoordinateSystem))
    }
}