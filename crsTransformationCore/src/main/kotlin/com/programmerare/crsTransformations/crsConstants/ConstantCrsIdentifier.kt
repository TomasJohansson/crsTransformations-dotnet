package com.programmerare.crsTransformations.crsConstants

import com.programmerare.crsTransformations.CrsIdentifier

// Regarding comments/documentation: See the file ConstantEpsgNumber

object ConstantCrsIdentifier {

    @JvmField
    val WGS84: CrsIdentifier = CrsIdentifier.createFromEpsgNumber(ConstantEpsgNumber.WGS84)

    @JvmField
    val SWEREF99TM      = CrsIdentifier.createFromEpsgNumber(ConstantEpsgNumber.SWEREF99TM)

    @JvmField
    val RT90_25_GON_V   = CrsIdentifier.createFromEpsgNumber(ConstantEpsgNumber.RT90_25_GON_V) // EPSG:3021: RT90 2.5 gon V		https://epsg.io/3021
}