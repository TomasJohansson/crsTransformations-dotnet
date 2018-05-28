package com.programmerare.crsCodeGeneration.constantsGenerator

class ConstantTypeNameValue(
        private val constantNameRenderer: ConstantNameRenderer,
        val epsgNumber: Int,
        private val areaName: String,
        private val crsName: String
//    val javadocCommentForConstant: String,
//    val dataTypeForConstant: DataType,
//    val nameForConstant: String,
//    val valueForConstant: String
)
{
    fun getJavadocCommentForConstant(): String {
        return "EPSG:" + epsgNumber + " , " + crsName + " , " + areaName
    }

    fun getValueForConstant(): String {
        return constantNameRenderer.getValueForConstant(epsgNumber)
    }

    fun getNameForConstant(): String {
        return constantNameRenderer.getNameForConstant(crsName, areaName, epsgNumber)
    }

    fun getDataTypeForConstant(): DataType {
        return constantNameRenderer.getDataTypeForConstant()
    }
}