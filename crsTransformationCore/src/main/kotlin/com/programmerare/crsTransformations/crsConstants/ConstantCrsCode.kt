package com.programmerare.crsTransformations.crsConstants

// Regarding comments/documentation: See the file ConstantEpsgNumber

object ConstantCrsCode {
    private const val EPSG_PREFIX = "EPSG:"

    const val WGS84         = EPSG_PREFIX + ConstantEpsgNumber.WGS84
    const val SWEREF99TM    = EPSG_PREFIX + ConstantEpsgNumber.SWEREF99TM
    const val RT90_25_GON_V = EPSG_PREFIX + ConstantEpsgNumber.RT90_25_GON_V // EPSG:3021: RT90 2.5 gon V		https://epsg.io/3021
}