@file:JvmName("StringUtils")
package com.programmerare.crsTransformations.extensionfunctions

import com.programmerare.crsTransformations.CrsTransformationFacade

fun String.createCrsTransformationFacadeFromFullClassName(): CrsTransformationFacade {
    val crsTransformationFacade = Class.forName(this).getDeclaredConstructor().newInstance() as CrsTransformationFacade
    return crsTransformationFacade
}