@file:JvmName("StringUtils") // TODO: this is not really a general StringUtils class so therefore move the method away from here ...
package com.programmerare.crsTransformations.extensionfunctions

import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterLeafFactory

/**
 * Utility method for creating an adapter instance by using reflection
 * for a string that should be the full class name for an implementation.
 * When using Kotlin the method is available as an extension method for strings,
 * but when using Java the method is available for a class named 'StringUtils'. 
 */
@Deprecated("Use the method now in the class CrsTransformationAdapterLeafFactory") // instead use the method in CrsTransformationAdapterLeafFactory
fun String.createCrsTransformationAdapterFromFullClassName(): CrsTransformationAdapter {
    // TODO use the below method directly instead
    return CrsTransformationAdapterLeafFactory.createCrsTransformationAdapterFromFullClassName2(this)
}