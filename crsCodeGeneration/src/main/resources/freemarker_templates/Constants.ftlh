package com.programmerare.crsConstants;

public class EpsgNumber {
	<#list constants as constant>
	public final static ${constant.dataTypeForConstant.dataTypeName} ${constant.nameForConstant} = ${constant.valueForConstant?no_esc};
	</#list>		
}