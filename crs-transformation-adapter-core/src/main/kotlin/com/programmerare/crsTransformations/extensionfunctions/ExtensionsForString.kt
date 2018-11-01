@file:JvmName("StringUtils")
package com.programmerare.crsTransformations.extensionfunctions

import com.programmerare.crsTransformations.CrsTransformationAdapter

fun String.createCrsTransformationAdapterFromFullClassName(): CrsTransformationAdapter {
    val crsTransformationAdapter = Class.forName(this).getDeclaredConstructor().newInstance() as CrsTransformationAdapter
    return crsTransformationAdapter
}