using System.Collections.Generic;
using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Identifier;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_8_9;
using System.IO;
using System;

namespace Programmerare.CrsTransformations.Core {
public class CrsTransformationAdapterParameterizedTests : CrsTransformationTestBase {

    private const double DELTA_VALUE_FOR_COMPARISONS_WITH_UNIT_METER = 1.0;
    private const double DELTA_VALUE_FOR_COMPARISONS_WITH_UNIT_DEGREES = 0.0001;

    // ...\Programmerare.CrsTransformations.Test\bin\Debug\netcoreapp2.1\resources\coordinatesForSweden.csv
    private const string testFileWithSomeCoordinatesForSweden = "resources/coordinatesForSweden.csv";

    private const int lowerEpsgIntervalForSwedishProjectionsUsingMeterAsUnit = EpsgNumber.SWEDEN__SWEREF99_TM__3006; // 3006;
    private const int upperEpsgIntervalForSwedishProjectionsUsingMeterAsUnit = EpsgNumber.SWEDEN__5_GON_E__RT90_5_GON_O__3024; // 3024;

    // When doing assertions, the delta value should depend on whether the units 
    // are meters or degrees (for degree latitudes the value "1" is very big but 
    // only one meter for the swedish projections with EPSG codes in the below list) 
    private List<int> epsgNumbersForSwedishProjectionsUsingMeterAsUnit;

    [SetUp]
    public void SetUpParameterizedTests() {
        //epsgNumbersForSwedishProjectionsUsingMeterAsUnit  = 
        //        Enumerable.Range(
        //            lowerEpsgIntervalForSwedishProjectionsUsingMeterAsUnit, 
        //            upperEpsgIntervalForSwedishProjectionsUsingMeterAsUnit
        //        ).ToList();
        epsgNumbersForSwedishProjectionsUsingMeterAsUnit = new List<int>();
        for(
            int i=lowerEpsgIntervalForSwedishProjectionsUsingMeterAsUnit;
            i<=upperEpsgIntervalForSwedishProjectionsUsingMeterAsUnit;
            i++
        ) {
            epsgNumbersForSwedishProjectionsUsingMeterAsUnit.Add(i);
        }
        Assert.AreEqual(
            7, // currently 3 leaf implementations and 4 composites
            base.crsTransformationAdapterImplementations.Count
        );
    }

    [TestCaseSource(nameof(GetCsvDataFromFileWithSomeCoordinatesForSweden))]
    public void VerifyTransformationsCorrespondToCsvFileCoordinates(
        TestDataSweden t
    ) {
        string description = t.description;
        double wgs84Lat = t.wgs84Lat;
        double wgs84Lon = t.wgs84Lon;
        double rt90north = t.rt90north;
        double rt90east = t.rt90east;
        double sweref99north = t.sweref99north;
        double sweref99east = t.sweref99east;
        string url = t.url;

        // example row from the csv file:
        // Stockholm Centralstation;59.330231;18.059196;6580994;1628294;6580822;674032;https://kartor.eniro.se/m/03Yxp

        // These used coordinates (i.e. those in the csv file) were manually retrieved from the Eniro
        // site at the URL's for each row, and by clicking the coordinate feature
        // which shows the coordinates in the three systems WGS84, RT90, SWREF99

        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            TransformToCoordinate_ShouldReturnEqualCoordinates_WhenTransformingBetweenTwoKnownCoordinatesToAndFromEachOther(
                crsTransformationAdapter,
                epsgNumberForWgs84, epsgNumberForRT90,
                wgs84Lat, wgs84Lon,
                rt90north, rt90east,
                description
            );

            TransformToCoordinate_ShouldReturnEqualCoordinates_WhenTransformingBetweenTwoKnownCoordinatesToAndFromEachOther(
                crsTransformationAdapter,
                epsgNumberForWgs84, epsgNumberForSweref99TM,
                wgs84Lat, wgs84Lon,
                sweref99north, sweref99east,
                description
            );

            TransformToCoordinate_ShouldReturnEqualCoordinates_WhenTransformingBetweenTwoKnownCoordinatesToAndFromEachOther(
                crsTransformationAdapter,
                epsgNumberForRT90, epsgNumberForSweref99TM,
                rt90north, rt90east,
                sweref99north, sweref99east,
                description
            );
        }
    }

    [TestCaseSource(nameof(GetCsvDataFromFileWithSomeCoordinatesForSweden))]
    public void VerifyTransformationsBackAndForthFromWgs84ToSwedishProjections(
        TestDataSweden t
    ) {
        CrsCoordinate inputCoordinateWGS84 = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(
            t.wgs84Lon, 
            t.wgs84Lat, 
            epsgNumberForWgs84
        );
        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            foreach (int epsgNumber in epsgNumbersForSwedishProjectionsUsingMeterAsUnit) {
                TransformToCoordinate_ShouldReturnTheOriginalCoordinate_WhenTransformingBackAgainFromTheResult(
                    crsTransformationAdapter,
                    inputCoordinateWGS84,
                    epsgNumber
                );
            }
        }
    }
    
    [TestCaseSource(nameof(GetCsvDataFromFileWithSomeCoordinatesForSweden))]
    public void TransformToCoordinate_ShouldReturnTheSameCoordinate_WhenTransformingWithDifferentImplementations(
        TestDataSweden t
    ) {
        CrsCoordinate inputCoordinateWGS84 = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(
            t.wgs84Lon, 
            t.wgs84Lat, 
            epsgNumberForWgs84
        );
        for (int i = 0; i < crsTransformationAdapterImplementations.Count-1; i++) {
            for (int j = i+1; j < crsTransformationAdapterImplementations.Count; j++) {
                foreach (int epsgNumber in epsgNumbersForSwedishProjectionsUsingMeterAsUnit) {
                    TransformToCoordinate_ShouldReturnTheSameCoordinate_WhenTransformingWithTwoDifferentImplementations(
                        crsTransformationAdapterImplementations[i],
                        crsTransformationAdapterImplementations[j],
                        inputCoordinateWGS84,
                        epsgNumber
                    );
                }
            }
        }
    }
    
    private void TransformToCoordinate_ShouldReturnEqualCoordinates_WhenTransformingBetweenTwoKnownCoordinatesToAndFromEachOther(
        ICrsTransformationAdapter crsTransformationAdapter,
        int epsgNumber1, int epsgNumber2,
        double yLat1, double xLon1,
        double yLat2, double xLon2,
        string descriptionPlace
    ) {
        string description = descriptionPlace + " , implementation: " + crsTransformationAdapter.AdapteeType;
        CrsCoordinate coordinate1 = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(xLon1, yLat1, epsgNumber1);
        CrsCoordinate coordinate2 = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(xLon2, yLat2, epsgNumber2);
        CrsCoordinate outputForCoordinate1 = crsTransformationAdapter.TransformToCoordinate(coordinate1, epsgNumber2);
        CrsCoordinate outputForCoordinate2 = crsTransformationAdapter.TransformToCoordinate(coordinate2, epsgNumber1);

        double delta = GetDeltaValueForComparisons(epsgNumber2);
        Assert.AreEqual(coordinate2.XEastingLongitude, outputForCoordinate1.XEastingLongitude, delta, description);
        Assert.AreEqual(coordinate2.YNorthingLatitude, outputForCoordinate1.YNorthingLatitude, delta, description);

        delta = GetDeltaValueForComparisons(epsgNumber1);
        Assert.AreEqual(coordinate1.XEastingLongitude, outputForCoordinate2.XEastingLongitude, delta, description);
        Assert.AreEqual(coordinate1.YNorthingLatitude, outputForCoordinate2.YNorthingLatitude, delta, description);
    }

    private void TransformToCoordinate_ShouldReturnTheOriginalCoordinate_WhenTransformingBackAgainFromTheResult(
        ICrsTransformationAdapter crsTransformationAdapter,
        CrsCoordinate inputCoordinateOriginalCRS,
        int epsgNumberForTransformTargetCRS
    ) {
        double delta = GetDeltaValueForComparisons(inputCoordinateOriginalCRS.CrsIdentifier);

        CrsCoordinate outputCoordinateForTransformTargetCRS = crsTransformationAdapter.TransformToCoordinate(inputCoordinateOriginalCRS, epsgNumberForTransformTargetCRS);
        CrsCoordinate outputCoordinateOriginalCRS = crsTransformationAdapter.TransformToCoordinate(outputCoordinateForTransformTargetCRS, inputCoordinateOriginalCRS.CrsIdentifier.EpsgNumber);

        Assert.AreEqual(inputCoordinateOriginalCRS.XEastingLongitude, outputCoordinateOriginalCRS.XEastingLongitude, delta);
        Assert.AreEqual(inputCoordinateOriginalCRS.YNorthingLatitude, outputCoordinateOriginalCRS.YNorthingLatitude, delta);
        Assert.AreEqual(inputCoordinateOriginalCRS.CrsIdentifier.EpsgNumber, outputCoordinateOriginalCRS.CrsIdentifier.EpsgNumber);
    }

    private void TransformToCoordinate_ShouldReturnTheSameCoordinate_WhenTransformingWithTwoDifferentImplementations(
        ICrsTransformationAdapter crsTransformationAdapter1,
        ICrsTransformationAdapter crsTransformationAdapter2,
        CrsCoordinate inputCoordinate,
        int epsgNumberForOutputCoordinate
    ) {
        double delta = GetDeltaValueForComparisons(epsgNumberForOutputCoordinate);

        CrsCoordinate outputCoordinate1 = crsTransformationAdapter1.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinate);
        CrsCoordinate outputCoordinate2 = crsTransformationAdapter2.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinate);

        //Supplier<String> errorMessageLongitude = () -> "delta used: " + delta + " and the diff was " + Math.Abs(outputCoordinate1.XEastingLongitude - outputCoordinate2.XEastingLongitude);
        //Supplier<String> errorMessageLatitude = () -> "delta used: " + delta + " and the diff was " + Math.Abs(outputCoordinate1.YNorthingLatitude - outputCoordinate2.YNorthingLatitude);
        //Assert.AreEqual(outputCoordinate1.XEastingLongitude(), outputCoordinate2.getXEastingLongitude(), delta, errorMessageLongitude);
        //Assert.AreEqual(outputCoordinate1.YNorthingLatitude(), outputCoordinate2.getYNorthingLatitude(), delta, errorMessageLatitude);
        //Assert.AreEqual(outputCoordinate1.CrsIdentifier().getEpsgNumber(), outputCoordinate2.getCrsIdentifier().getEpsgNumber());
        string errorMessageLongitude = "delta used: " + delta + " and the diff was " + Math.Abs(outputCoordinate1.XEastingLongitude - outputCoordinate2.XEastingLongitude);
        string  errorMessageLatitude = "delta used: " + delta + " and the diff was " + Math.Abs(outputCoordinate1.YNorthingLatitude - outputCoordinate2.YNorthingLatitude);
        Assert.AreEqual(outputCoordinate1.XEastingLongitude, outputCoordinate2.XEastingLongitude, delta, errorMessageLongitude);
        Assert.AreEqual(outputCoordinate1.YNorthingLatitude, outputCoordinate2.YNorthingLatitude, delta, errorMessageLatitude);
        Assert.AreEqual(outputCoordinate1.CrsIdentifier.EpsgNumber, outputCoordinate2.CrsIdentifier.EpsgNumber);
    }

    private double GetDeltaValueForComparisons(
        CrsIdentifier crsIdentifier
    ) {
        return GetDeltaValueForComparisons(crsIdentifier.EpsgNumber);
    }

    private double GetDeltaValueForComparisons(
        int epsgNumber
    ) {
        CoordinateReferenceSystemUnit coordinateReferenceSystemUnit = CoordinateReferenceSystemUnit.UNKNOWN;
        if(epsgNumber == epsgNumberForWgs84) {
            coordinateReferenceSystemUnit = CoordinateReferenceSystemUnit.DEGREES;
        }
        // sweref : 3006 - 3018
        // RT90 :   3019 - 3024
        else if( // if(epsgNumber >= 3006 && epsgNumber <= 3024)
            lowerEpsgIntervalForSwedishProjectionsUsingMeterAsUnit <= epsgNumber
            &&
            epsgNumber <= upperEpsgIntervalForSwedishProjectionsUsingMeterAsUnit
        ) {
            coordinateReferenceSystemUnit = CoordinateReferenceSystemUnit.METERS;
        }
        return GetDeltaValueForComparisons(coordinateReferenceSystemUnit, epsgNumber);
    }

    private double GetDeltaValueForComparisons(
        CoordinateReferenceSystemUnit coordinateReferenceSystemUnit,
        int epsgNumberUsedOnlyInErrorMessage
    ) {
        if(coordinateReferenceSystemUnit == CoordinateReferenceSystemUnit.DEGREES) {
            return DELTA_VALUE_FOR_COMPARISONS_WITH_UNIT_DEGREES;
        }
        else if(coordinateReferenceSystemUnit == CoordinateReferenceSystemUnit.METERS) {
            return DELTA_VALUE_FOR_COMPARISONS_WITH_UNIT_METER;
        }
        else { // if(coordinateReferenceSystemUnit == CoordinateReferenceSystemUnit.UNKNOWN) {
            throw new ArgumentException("Not supported epsg number: " + epsgNumberUsedOnlyInErrorMessage);
        }
    }

    private enum CoordinateReferenceSystemUnit {
        DEGREES,
        METERS,
        UNKNOWN
    }

    [Test]
    public void VerifyPopulationOfListWithEpsgNumbers() {
        Assert.IsNotNull(epsgNumbersForSwedishProjectionsUsingMeterAsUnit);
        int differecenBetweenUpperAndLowerValue = upperEpsgIntervalForSwedishProjectionsUsingMeterAsUnit - lowerEpsgIntervalForSwedishProjectionsUsingMeterAsUnit;
        Assert.AreEqual(
            1 + differecenBetweenUpperAndLowerValue
            ,
            epsgNumbersForSwedishProjectionsUsingMeterAsUnit.Count
        );
        Assert.AreEqual(
            lowerEpsgIntervalForSwedishProjectionsUsingMeterAsUnit
            ,
            epsgNumbersForSwedishProjectionsUsingMeterAsUnit[0]
        );
        Assert.AreEqual(
            upperEpsgIntervalForSwedishProjectionsUsingMeterAsUnit
            ,
            epsgNumbersForSwedishProjectionsUsingMeterAsUnit[
                epsgNumbersForSwedishProjectionsUsingMeterAsUnit.Count-1
            ]
        );
    }

    private static IEnumerable<TestDataSweden> GetCsvDataFromFileWithSomeCoordinatesForSweden() {
        using(StreamReader sr = File.OpenText(testFileWithSomeCoordinatesForSweden)) {
            string line;
            while( (line = sr.ReadLine()) != null ) {
                // the first line of the file (which will be ignored in this iteration):
                //Description;4326Y;4326X;3021Y;3021X;3006Y;3006X;Url with some information about the location at the row
                if(
                    line.StartsWith("Description;") // the line above
                    || line.StartsWith("#") // "comment rows" to be ignored starts with a "#"
                ) continue;
                // example of a later line in the file i.e. one of those lines to become used:
                // Stockholm Centralstation;59.330231;18.059196;6580994;1628294;6580822;674032;https://kartor.eniro.se/m/03Yxp
                var arr = line.Split(";");
                var t = new TestDataSweden();
                t.description = arr[0];
                t.wgs84Lat = double.Parse(arr[1]);
                t.wgs84Lon = double.Parse(arr[2]);
                t.rt90north = double.Parse(arr[3]);
                t.rt90east = double.Parse(arr[4]);
                t.sweref99north = double.Parse(arr[5]);
                t.sweref99east = double.Parse(arr[6]);
                t.url = arr[7];
                yield return t;
            }
        }
    }
} // the test class ends here

public struct TestDataSweden {
    public string description;
    public double wgs84Lat, wgs84Lon;
    public double rt90north, rt90east;
    public double sweref99north, sweref99east;
    public string url;
}

} // namespace ends here