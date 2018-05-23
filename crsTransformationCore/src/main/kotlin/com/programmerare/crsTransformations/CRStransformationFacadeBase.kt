package com.programmerare.crsTransformations

abstract class CRStransformationFacadeBase : CRStransformationFacade {
    override final fun transform(inputCoordinate: Coordinate, crsCodeForOutputCoordinateSystem: String): Coordinate {
        // the below invoked overloaded method is implemented in subclasses
        // i.e. it is a hook method invoked by this Template Method
        return transform(inputCoordinate, CrsIdentifier.createFromCrsCode(crsCodeForOutputCoordinateSystem))
    }

    override final fun transform(inputCoordinate: Coordinate, epsgNumberForOutputCoordinateSystem: Int): Coordinate {
        // this Template Method is invokign the below overloaded hook method in subclasses
        return transform(inputCoordinate, CrsIdentifier.createFromEpsgNumber(epsgNumberForOutputCoordinateSystem))
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