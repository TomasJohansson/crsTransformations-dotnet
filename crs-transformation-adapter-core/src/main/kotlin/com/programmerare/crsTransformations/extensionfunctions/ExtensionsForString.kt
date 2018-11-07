@file:JvmName("StringUtils")
package com.programmerare.crsTransformations.extensionfunctions

import com.programmerare.crsTransformations.CrsTransformationAdapter

/**
 * Utility method for creating an adapter instance by using reflection
 * for a string that should be the full class name for an implementation.
 * When using Kotlin the method is available as an extension method for strings,
 * but when using Java the method is available for a class named 'StringUtils'. 
 */
fun String.createCrsTransformationAdapterFromFullClassName(): CrsTransformationAdapter {
    val crsTransformationAdapter = Class.forName(this).getDeclaredConstructor().newInstance() as CrsTransformationAdapter
    return crsTransformationAdapter
}