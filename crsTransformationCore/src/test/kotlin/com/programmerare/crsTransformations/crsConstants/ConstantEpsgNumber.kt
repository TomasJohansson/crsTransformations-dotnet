package com.programmerare.crsTransformations.crsConstants

// TODO: Generate these three files with constants, from some data store with lots of codes:
// ConstantEpsgNumber (integers)
// ConstantCrsCode (strings with the same numbers but "EPSG:" prefix)
// ConstantCrsIdentifier (CrsIdentifier objects)

// Also to consider: Dependencies and packaging i.e. where should these constants be located:
// Maybe the two constant classes with integers and strings should be a separate
// library with no depenency in either direction.
// If it would be desirable to use it, then the user need to explicit
// declare the Maven/Gradle depenencies for the library with those constants.

// Regarding the third file with "constants" to CrsIdentifier objects,
// it must of course have a dependenct to the core package.
// One possibility is to simply include those constants within the core package.
// However, if it would be very big with thousands of constants,
// maybe package it into a separate library which the end user will have
// to retrieve explicitly if desired ...

object ConstantEpsgNumber {
    const val WGS84 = 4326
    const val SWEREF99TM = 3006
    const val RT90_25_GON_V = 3021 // EPSG:3021: RT90 2.5 gon V		https://epsg.io/3021

    const val SWEREF99_12_00 = 3007 // EPSG:3007: SWEREF99 12 00	https://epsg.io/3007
    const val SWEREF99_15_00 = 3009 // EPSG:3009: SWEREF99 15 00	https://epsg.io/3009

    const val RT90_05_GON_O = 3024 // EPSG:3024: RT90 5 gon O       https://epsg.io/3024
}