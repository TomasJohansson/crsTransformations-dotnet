package com.programmerare.crsCodeGeneration.constantsGenerator

enum class DataType(val dataTypeName: String) {
    INTEGER("int"),
    STRING_JAVA("String"), // uppercased first letter
    STRING_CSHARPE("string") // lowercased first letter
}