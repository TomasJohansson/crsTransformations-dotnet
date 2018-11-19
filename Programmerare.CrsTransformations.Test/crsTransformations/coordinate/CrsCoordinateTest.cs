namespace com.programmerare.crsTransformations.coordinate {

// TODO: method overloading with F# instead of having different 
// method name suffixes ... e.g. "Epsg" or "CrsCode" as method name suffix

using com.programmerare.crsTransformations.crsIdentifier;
using NUnit.Framework;
using System;

[TestFixture]
class CrsCoordinateTest {
    private const string EpsgPrefix = "EPSG:";
    
    private const double deltaTolerance = 0.00001;
    private const double xLongitude = 12.34;
    private const double yLatitude = 56.67;

    // TODO: include the constants class which currently is still located 
    // in a directory not included in the Visual Studio Solution
    // ( JVM_project_to_become_ported_to_dotnet\crs-transformation-constants  ) 
    private const int epsgNumber = 3006;//EpsgNumber.SWEDEN__SWEREF99_TM__3006;
    private const int epsgNumberWgs84 = 4326; //EpsgNumber.WORLD__WGS_84__4326,

    private const string epsgCode = "EPSG:3006";//EpsgPrefix + epsgNumber;// EpsgCode._3006__SWEREF99_TM__SWEDEN;

    [Test]
    public void coordinateProperties_shouldHaveValuesEqualtToFactoryMethodsParameters() {
        CrsCoordinate coordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitudeEpsg(xLongitude, yLatitude, epsgNumber);
        Assert.AreEqual(xLongitude, coordinate.XEastingLongitude, deltaTolerance);
        Assert.AreEqual(yLatitude, coordinate.YNorthingLatitude, deltaTolerance);
        Assert.AreEqual(epsgNumber, coordinate.CrsIdentifier.EpsgNumber);
    }

    [Test]
    public void coordinates_shouldBeEqual_whenUsingIntegerEpsgNumberAndDifferentFactoryMethodsWithParametersInDifferentOrder() {
        CrsCoordinate coordinate1 = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitudeEpsg(xLongitude, yLatitude, epsgNumber);
        CrsCoordinate coordinate2 = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitudeEpsg(yLatitude, xLongitude, epsgNumber);
        AssertEqualCoordinates(coordinate1, coordinate2);
    }

    [Test]
    public void coordinates_shouldBeEqual_whenUsingStringEpsgCodeAndDifferentFactoryMethodsWithParametersInDifferentOrder() {
        String crsCode = "EPSG:3006";
        CrsCoordinate coordinate1 = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitudeCrsCode(xLongitude, yLatitude, crsCode);
        CrsCoordinate coordinate2 = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitudeCrsCode(yLatitude, xLongitude, crsCode);
        AssertEqualCoordinates(coordinate1, coordinate2);
    }

    [Test]
    public void coordinates_shouldBeEqual_whenUsingCrsIdentifierAndDifferentFactoryMethodsWithParametersInDifferentOrder() {
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.CreateFromEpsgNumber(3006);
        CrsCoordinate coordinate1 = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(xLongitude, yLatitude, crsIdentifier);
        CrsCoordinate coordinate2 = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(yLatitude, xLongitude, crsIdentifier);
        AssertEqualCoordinates(coordinate1, coordinate2);
    }

    [Test]
    public void coordinate_shouldBeCreatedWithWGS84asDefaultCrs_whenNotSpecifyingCrs() {
        CrsCoordinate coordinate;
        
        coordinate = CrsCoordinateFactory.CreateFromLongitudeLatitude(xLongitude, yLatitude);
        Assert.AreEqual(
            epsgNumberWgs84, // EpsgNumber.WORLD__WGS_84__4326,
            coordinate.CrsIdentifier.EpsgNumber
        );

        // The tets below is the same as above except that the factory method use the reversed order for lat/lon parameters
        coordinate = CrsCoordinateFactory.CreateFromLatitudeLongitude(yLatitude, xLongitude);
        Assert.AreEqual(
            epsgNumberWgs84, // EpsgNumber.WORLD__WGS_84__4326,
            coordinate.CrsIdentifier.EpsgNumber
        );        
    }

    private void AssertEqualCoordinates(CrsCoordinate coordinate1, CrsCoordinate coordinate2) {
        Assert.AreEqual(coordinate1.XEastingLongitude, coordinate2.XEastingLongitude, deltaTolerance);
        Assert.AreEqual(coordinate1.YNorthingLatitude, coordinate2.YNorthingLatitude, deltaTolerance);
        Assert.AreEqual(coordinate1.CrsIdentifier, coordinate2.CrsIdentifier); // data class
        Assert.AreEqual(coordinate1.CrsIdentifier.EpsgNumber, coordinate2.CrsIdentifier.EpsgNumber);

        Assert.AreEqual(coordinate1.GetHashCode(), coordinate2.GetHashCode());
        Assert.AreEqual(coordinate1, coordinate2);
    }

     // six decimals are commonly used for latitude and longitude values 
    [Test]
    public void coordinateWithSixDecimals_shouldBeEqualToCoordinateConstructedWithTheSameValues_whenTheOnlyDifferenceIsSomeAdditionalZeroes() {
        CrsCoordinate c1 = CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456, 18.123456000);
        CrsCoordinate c2 = CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456000, 18.123456);
        Assert.AreEqual(
            c1, c2
        );
        Assert.AreEqual(
            c1.GetHashCode(), c2.GetHashCode()
        );
    }

    [Test]
    public void coordinateWithNineDecimals_shouldBeEqualToCoordinateConstructedWithTheSameValues_whenTheOnlyDifferenceIsSomeAdditionalZeroes() {
        CrsCoordinate c1 = CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456789, 18.123456789000);
        CrsCoordinate c2 = CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456789000, 18.123456789);
        Assert.AreEqual(
            c1, c2
        );
        Assert.AreEqual(
            c1.GetHashCode(), c2.GetHashCode()
        );
    }

    [Test]
    public void coordinates_shouldNotBeEqual_whenDifferenceAtTwelfthDecimalOfLatitude()
    {
        // very small latitude difference:
        Assert.AreNotEqual(
            CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456789001, 18.123456789),
            CrsCoordinateFactory.CreateFromLatitudeLongitude(49.123456789002, 18.123456789)
        );
    }

    [Test]
    public void coordinates_shouldNotBeEqual_whenDifferenceAtTwelfthDecimalOfLongitude() {
        // very small longitude difference:
        Assert.AreNotEqual(
            CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456789, 18.123456789000),
            CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456789, 18.123456789001)
        );
    }

    [Test]
    public void coordinates_shouldBeEqual_whenCreatedWithTheSameValuesButDifferentFactoryMethods() {
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumber);
        string epsgCode = EpsgPrefix + epsgNumber;
        
        CrsCoordinate expectedCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitudeEpsg(xLongitude, yLatitude, epsgNumber);
        // all should be equal to each other, so one of them was chosen above 
        // as the "expected" and then the others are compared with it in the below assertions

        // -----------------------------------------------------------------------
        // the last parameter (epsgNumber) is an integer in the first below assertions:
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LonLatEpsg(xLongitude, yLatitude, epsgNumber)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.XYepsg(xLongitude, yLatitude, epsgNumber)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.EastingNorthingEpsg(xLongitude, yLatitude, epsgNumber)
        );

        // the below four assertions are using x/y values in the opposite order 
        // compared to the above three assertions
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitudeEpsg(yLatitude, xLongitude, epsgNumber)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LatLonEpsg(yLatitude, xLongitude, epsgNumber)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.YXepsg(yLatitude, xLongitude, epsgNumber)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.NorthingEastingEpsg(yLatitude, xLongitude, epsgNumber)
        );

        // -----------------------------------------------------------------------
        // epsg code (string parameter) is the last parameter below
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitudeCrsCode(xLongitude, yLatitude, epsgCode)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LonLatCrsCode(xLongitude, yLatitude, epsgCode)   
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.XYcrsCode(xLongitude, yLatitude, epsgCode)   
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.EastingNorthingCrsCode(xLongitude, yLatitude, epsgCode)  
        );

        // the below four assertions are using x/y values in the opposite order 
        // compared to the above four assertions

        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitudeCrsCode(yLatitude, xLongitude, epsgCode)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LatLonCrsCode(yLatitude, xLongitude, epsgCode)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.YXcrsCode(yLatitude, xLongitude, epsgCode)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.NorthingEastingCrsCode(yLatitude, xLongitude, epsgCode)
        );        

        // -----------------------------------------------------------------------
        // crsIdentifier obkect is the last parameter below
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(xLongitude, yLatitude, crsIdentifier)   
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LonLatCrs(xLongitude, yLatitude, crsIdentifier)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.XY(xLongitude, yLatitude, crsIdentifier)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.EastingNorthing(xLongitude, yLatitude, crsIdentifier)
        );

        // the below four assertions are using x/y values in the opposite order 
        // compared to the above four assertions

        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(yLatitude, xLongitude, crsIdentifier)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LatLonCrs(yLatitude, xLongitude, crsIdentifier)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.YX(yLatitude, xLongitude, crsIdentifier)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.NorthingEasting(yLatitude, xLongitude, crsIdentifier)
        );        
    }

    [Test]
    public void coordinate_shouldHaveEquivalentXEastingLongitudeProperties() {
        CrsCoordinate c = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitudeCrsCode(yLatitude, xLongitude, epsgCode);
        Assert.AreEqual(c.XEastingLongitude, c.X);
        Assert.AreEqual(c.XEastingLongitude, c.Easting);
        Assert.AreEqual(c.XEastingLongitude, c.Longitude);
    }

    [Test]
    public void coordinate_shouldHaveEquivalentgetYNorthingLatitudeProperties() {
        CrsCoordinate c = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitudeCrsCode(yLatitude, xLongitude, epsgCode);
        Assert.AreEqual(c.YNorthingLatitude, c.Y);
        Assert.AreEqual(c.YNorthingLatitude, c.Northing);
        Assert.AreEqual(c.YNorthingLatitude, c.Latitude);
    }

    // throw IllegalArgumentException("Neither of the two coordinate parameters must be null i.e. neither 'X / Easting / Longitude' nor 'Y / Northing / Latitude'")
    // fragile hardcoded string below but will not change often and if/when then it will be easy to fix when it fails
    private const string EXPECTED_PART_OF_EXCEPTION_MESSAGE_WHEN_COORDINATE_VALUES_IS_NULL = "Neither of the two coordinate parameters must be null";


    [Test]
    public void createFromXEastingLongitudeAndYNorthingLatitude_shouldThrowException_whenCrsIdentifierIsNull()
    {
        CrsIdentifier crsIdentifier = null;
        ArgumentException exception = Assert.Throws<ArgumentNullException>( () => {
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(
                60.0,
                20.0,
                crsIdentifier
            );
        });
        // F# may throw this: nullArg "crsIdentifier"
        // which creates the following kind of message:
        // "Value cannot be null. Parameter name: crsIdentifier"
        AssertExceptionMessageForIllegalArgumentException(exception, "crsIdentifier");
    }

    [Test]
    public void createFromXEastingLongitudeAndYNorthingLatitude_shouldThrowException_whenCrsCodeIsNull()
    {
        string crsCode = null;
        ArgumentException exception = Assert.Throws<ArgumentNullException>( () => {
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitudeCrsCode(
                60.0,
                20.0,
                crsCode
            );
        });
        // F# may throw this: nullArg "crsCode"
        // which creates the following kind of message:
        // "Value cannot be null. Parameter name: crsCode"
        AssertExceptionMessageForIllegalArgumentException(exception, "crsCode");
    }

    private void AssertExceptionMessageForIllegalArgumentException(
        ArgumentException exception, 
        string suffixWithNameOfParameter
    ) {
        Assert.NotNull(exception);
        string actualEceptionMessage = exception.Message;
        // fragile hardcoded strings below but will not change often and if/when then it will be easy to fix when it fails
        // actualEceptionMessage for example: "Value cannot be null. Parameter name: crsIdentifier"
        //string expectedEceptionMessagePart1 = "Parameter specified as non-null is null:";
        //string expectedEceptionMessagePart2 = "parameter " + suffixWithNameOfParameter;
        //Assert.That(exception.Message, Does.Contain(expectedEceptionMessagePart1));
        //Assert.That(exception.Message, Does.Contain(expectedEceptionMessagePart2));
        Assert.That(exception.Message, Does.Contain(suffixWithNameOfParameter));
    }
} // class ends
} // namespace ends