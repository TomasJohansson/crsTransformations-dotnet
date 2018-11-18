package com.programmerare.crsTransformations.coordinate

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.*

internal class CrsCoordinateKotlinTest {

    // Most tests are creted with Java e.g. CrsCoordinateTest
    // but this test method below was created just to illistrate 
    // the difference, i.e. with Kotlin it is more difficult
    // to create a KotlinNullPointerException by accident
    // since you must type a variable parameter as nullable
    // to make it possible to set it to null, and when
    // a method parameter is not typed as nullable
    // you need to use something like below with "!!"
    
    @Test
    fun createFromXEastingLongitudeAndYNorthingLatitude_shouldThrowException_whenYisNull() {
        // val y: Double = null // not possible with Kotlin but need to use "Double?" as below
        val y: Double? = null
        
        val exception = assertThrows(
            Throwable::class.java
        ) {
            createFromXEastingLongitudeAndYNorthingLatitude(
                60.0,

                // y, 
                // "y" as above does not even compile with Kotlin when the 
                // formal parameter is typed as "Double" instead of "Double?"
                // and instead we have to do something like below:
                y!!,
                    
                4326
            )
        }
        // this is currently creating a "kotlin.KotlinNullPointerException"
        // and it is preferable with a better message
    }   
}