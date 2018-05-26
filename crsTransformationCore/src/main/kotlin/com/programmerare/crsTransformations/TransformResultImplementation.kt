package com.programmerare.crsTransformations

import java.lang.IllegalStateException
import java.lang.RuntimeException

class TransformResultImplementation(
    override val inputCoordinate: Coordinate,
    outputCoordinate: Coordinate?,
    override val exception: Exception?,
    override val isSuccess: Boolean,
    override val crsTransformationFacadeThatCreatedTheResult: CrsTransformationFacade
): TransformResult {

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
}