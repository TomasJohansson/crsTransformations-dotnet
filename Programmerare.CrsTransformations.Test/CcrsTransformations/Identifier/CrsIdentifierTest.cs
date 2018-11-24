namespace Programmerare.CrsTransformations.Identifier {

using NUnit.Framework;
using System;

[TestFixture]
public class CrsIdentifierTest {

    private String GetCrsCodeIncludingUppercasedEpsgPrefix(int epsgNumber) {
        return "EPSG:" + epsgNumber;
    }
    
    [Test]
    public void crsIdentifier_shouldReturnWhitespaceTrimmedCrsCodeAndNotBeConsideredAsEpsg_whenCreatedFromNonEpsgString() {
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.CreateFromCrsCode("  abc  ");
        Assert.AreEqual("abc", crsIdentifier.CrsCode);
        Assert.AreEqual(false, crsIdentifier.IsEpsgCode);
    }

    [Test]
    public void crsIdentifier_shouldReturnEpsgNumberAndEpsgPrefixedCrsCodeAndBeConsideredAsEpsg_whenCreatedFromEpsgNumber() {
        int inputEpsgNumber = 3006;
        // No validation that the number is actually an existing EPSG but any positive integer
        // is assumed to be a EPSG number
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.CreateFromEpsgNumber(inputEpsgNumber);
        Assert.AreEqual(
            inputEpsgNumber, // expected
            crsIdentifier.EpsgNumber
        );
        Assert.AreEqual(GetCrsCodeIncludingUppercasedEpsgPrefix(inputEpsgNumber), crsIdentifier.CrsCode);
        Assert.AreEqual(true, crsIdentifier.IsEpsgCode);
    }

    [Test]
    public void crsIdentifier_shouldReturnEpsgNumberAndUppercasedEpsgPrefixedWhitespaceTrimmedCrsCodeAndBeConsideredAsEpsg_whenCreatedFromLowecasedEpsgCodeWithSurroundingWhitespace() {
        int inputEpsgNumber = 4326;
        String inputCrsCode = "  epsg:" + inputEpsgNumber + "  "; 
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.CreateFromCrsCode(inputCrsCode);
        // the input should become trimmed and return string with uppercased "EPSG:" prefix
        Assert.AreEqual(
            GetCrsCodeIncludingUppercasedEpsgPrefix(inputEpsgNumber), 
            crsIdentifier.CrsCode
        );
        Assert.AreEqual(true, crsIdentifier.IsEpsgCode);
        Assert.AreEqual(inputEpsgNumber, crsIdentifier.EpsgNumber);
    }

    [Test]
    public void crsIdentifierFactory_shouldThrowException_whenCrsCodeInputIsNull()
    {
        ArgumentException exception = Assert.Throws<ArgumentNullException>( () => {
            CrsIdentifierFactory.CreateFromCrsCode(null); // should fail
        }, "Must not be null");
        // F# code invoked above may throw exception like this:
        //  nullArg "crsCode"
        // Resulting message: "Value cannot be null. Parameter name: crsCode"
        AssertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "cannot be null");
    }

    [Test]
    public void crsIdentifierFactory_shouldThrowException_whenCrsCodeInputIsOnlyWhitespace()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>( () => {
            CrsIdentifierFactory.CreateFromCrsCode("   "); // should fail
        }, "Must not be empty string");
        AssertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "non-empty");
    }

    [Test]
    public void crsIdentifierFactory_shouldThrowException_whenCrsCodeIsEpsgWithNegativeNumber()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>( () => {
            CrsIdentifierFactory.CreateFromCrsCode(GetCrsCodeIncludingUppercasedEpsgPrefix(-123)); // should fail
        }, "EPSG must not be negative");
        AssertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "non-positive");
    }


    [Test]
    public void crsIdentifierFactory_shouldThrowException_epsgNumberIsNegative()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>( () => {
            CrsIdentifierFactory.CreateFromEpsgNumber(-1); // should fail
        }, "EPSG must not be negative");
        AssertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "non-positive");
    }

    [Test]
    public void crsIdentifiers_shouldBeEqual_whenCreatedFromEpsgNumberAndCorrespondingCrsCode()
    {
        CrsIdentifier fromEpsgNumber = CrsIdentifierFactory.CreateFromEpsgNumber(3006);
        CrsIdentifier fromCrsCode = CrsIdentifierFactory.CreateFromCrsCode("  epsg:3006   ");
        Assert.AreEqual(fromEpsgNumber, fromCrsCode);
        Assert.AreEqual(fromEpsgNumber.GetHashCode(), fromCrsCode.GetHashCode());
    }

    ///**
    // * @param exception
    // * @param expectedStringToBeContainedInExceptionMessage e.g. "non-null" or "non-empty"
    // */
    private void AssertExceptionMessageWhenArgumentWasNullOrEmptyString(
        ArgumentException exception,
        String expectedStringToBeContainedInExceptionMessage
    )
    {
        Assert.NotNull(exception);
        Assert.NotNull(exception.Message);
        // the exception message is currently something like this: "Parameter specified as non-null is null: method Programmerare.CrsTransformations.Identifier.CrsIdentifier$Companion.createFromCrsCode, parameter crsCode"
        // (potentially fragile to test the message strings but it does not really change often, and in such a rare scenario, then easy to fix)
        Assert.That(exception.Message, Does.Contain(expectedStringToBeContainedInExceptionMessage));
        // Alternative to the above test
        StringAssert.Contains(expectedStringToBeContainedInExceptionMessage, exception.Message);
    }

    [Test]
    public void createFromCrsCode_shouldThrowException_whenCrsCodeIsNull()
    {
        ArgumentException exception = Assert.Throws<ArgumentNullException>( () => {
            CrsIdentifierFactory.CreateFromCrsCode(null);
        }, "CRS code must not be null");
    }

    [Test]
    public void crsIdentifier_shouldNotBeEqual_whenDifferentEpsgNumber()
    {
        Assert.AreNotEqual(
            CrsIdentifierFactory.CreateFromEpsgNumber(123),
            CrsIdentifierFactory.CreateFromEpsgNumber(124)
        );
    }

    [Test]
    public void crsIdentifier_shouldNotBeEqual_whenDifferentCrsCode()
    {
        Assert.AreNotEqual(
            CrsIdentifierFactory.CreateFromCrsCode("EPSG:987"),
            CrsIdentifierFactory.CreateFromCrsCode("EPSG:986")
        );
    }

    [Test]
    public void crsIdentifier_shouldNotBeEqual_whenDifferentNonEpsgCrsCode()
    {
        Assert.AreNotEqual(
            CrsIdentifierFactory.CreateFromCrsCode("abc"),
            CrsIdentifierFactory.CreateFromCrsCode("abd")
        );
    }

} // class ends
} // namespace ends