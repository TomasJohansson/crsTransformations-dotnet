package ${nameOfJavaPackage};

public class ${nameOfJavaClass} {
    <#t>
	<#list constants as constant>
	
	/**
	* ${constant.javadocCommentForConstant}
	*/	
	public final static ${constant.dataTypeForConstant.dataTypeName} ${constant.nameForConstant} = ${constant.valueForConstant?no_esc};
	</#list>		
}