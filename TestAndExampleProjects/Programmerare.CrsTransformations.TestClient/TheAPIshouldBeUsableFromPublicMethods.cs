using System.Collections.Generic;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Identifier;
using Programmerare.CrsTransformations.CompositeTransformations;
using System;

namespace Programmerare.CrsTransformations.TestClient {
    // The "internal" methods in the F# code have been exposed 
    // for the C# test project to be able to use them.
    // Just to make sure that there are not too many methods 
    // now being internal instead of public, the code below should compile.
    // The code is tested in the test project, so no assertions here in this class below.
    // Being possible to compile is enough here.

    class TheAPIshouldBeUsableFromPublicMethods {
        private void crsIdentifierCode() {
            // public factory methods:
            crsIdentifier = CrsIdentifierFactory.CreateFromEpsgNumber(4326);
            crsIdentifier = CrsIdentifierFactory.CreateFromCrsCode("EPSG:4326");

            // public properties:
            crsCode = crsIdentifier.CrsCode;
            epsgNumber = crsIdentifier.EpsgNumber;
            isEpsgCode = crsIdentifier.IsEpsgCode;
        }

        private void crsCoordinateCode() {
            double x = 20.0, y = 20.0;
            
            // public factory methods:

            // two methods with implicit CRS WGS84 i.e. no third parameter:
            crsCoordinate = CrsCoordinateFactory.CreateFromLongitudeLatitude(x, y);
            crsCoordinate = CrsCoordinateFactory.CreateFromLatitudeLongitude(y, x);

            // Eight factory methods with CrsIdentifier as third parameter
            // (the first four with "x" first and then "y", and then four with "y" and "x" in opposite order)
            crsCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(x, y, crsIdentifier);
            crsCoordinate = CrsCoordinateFactory.XY(x, y, crsIdentifier);
            crsCoordinate = CrsCoordinateFactory.EastingNorthing(x, y, crsIdentifier);
            crsCoordinate = CrsCoordinateFactory.LonLat(x, y, crsIdentifier);
            
            crsCoordinate = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(y, x, crsIdentifier);
            crsCoordinate = CrsCoordinateFactory.YX(y, x, crsIdentifier);
            crsCoordinate = CrsCoordinateFactory.NorthingEasting(y, x, crsIdentifier);
            crsCoordinate = CrsCoordinateFactory.LatLon(y, x, crsIdentifier);

            // Eight factory methods with integer (epsgNumber) as third parameter
            // (the first four with "x" first and then "y", and then four with "y" and "x" in opposite order)            
            crsCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(x, y, epsgNumber);
            crsCoordinate = CrsCoordinateFactory.XY(x, y, epsgNumber);
            crsCoordinate = CrsCoordinateFactory.EastingNorthing(x, y, epsgNumber);
            crsCoordinate = CrsCoordinateFactory.LonLat(x, y, epsgNumber);

            crsCoordinate = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(y, x, epsgNumber);
            crsCoordinate = CrsCoordinateFactory.YX(y, x, epsgNumber);
            crsCoordinate = CrsCoordinateFactory.NorthingEasting(y, x, epsgNumber);
            crsCoordinate = CrsCoordinateFactory.LatLon(y, x, epsgNumber);

            // Eight factory methods with string (crsCode) as third parameter
            // (the first four with "x" first and then "y", and then four with "y" and "x" in opposite order)            
            crsCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(x, y, crsCode);
            crsCoordinate = CrsCoordinateFactory.XY(x, y, crsCode);
            crsCoordinate = CrsCoordinateFactory.EastingNorthing(x, y, crsCode);
            crsCoordinate = CrsCoordinateFactory.LonLat(x, y, crsCode);

            crsCoordinate = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(y, x, crsCode);
            crsCoordinate = CrsCoordinateFactory.YX(y, x, crsCode);
            crsCoordinate = CrsCoordinateFactory.NorthingEasting(y, x, crsCode);
            crsCoordinate = CrsCoordinateFactory.LatLon(y, x, crsCode);

            // nine public properties:
            crsIdentifier = crsCoordinate.CrsIdentifier;
            
            xEastingLongitude = crsCoordinate.XEastingLongitude;
            xEastingLongitude = crsCoordinate.X;
            xEastingLongitude = crsCoordinate.Easting;
            xEastingLongitude = crsCoordinate.Longitude;
            
            yNorthingLatitude = crsCoordinate.YNorthingLatitude;
            yNorthingLatitude = crsCoordinate.Y;
            yNorthingLatitude = crsCoordinate.Northing;
            yNorthingLatitude = crsCoordinate.Latitude;
        }
        
        private void crsTransformationAdapterCode() {
            // The three (currently) 'Leaf' adapters:
            crsTransformationAdapter = new CrsTransformationAdapterDotSpatial();
            crsTransformationAdapter = new CrsTransformationAdapterProjNet();
            crsTransformationAdapter = new CrsTransformationAdapterMightyLittleGeodesy();

            crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create();
            // Three factory methods for 'composite' adapters
            // (trying to create them all with reflection, and thus no parameter)
            crsTransformationAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian();
            crsTransformationAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage();
            crsTransformationAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess();

            // a list used as parameter for the below 'Composite' adapters:
            IList<ICrsTransformationAdapter> crsTransformationAdapters = new List<ICrsTransformationAdapter>{crsTransformationAdapter};
            // Three factory methods for 'composite' adapters
            // (with the specified leaf adapters in a list parameter)
            crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create(crsTransformationAdapters);
            crsTransformationAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian();
            crsTransformationAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage();
            crsTransformationAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess();

            // leaf adapters (used below for creating a composite with weighted average):
            var crsTransformationAdapterDotSpatial = new CrsTransformationAdapterDotSpatial();
            var crsTransformationAdapterProjNet = new CrsTransformationAdapterProjNet();
            // One of the above are used below, with the instance,
            // and one of them is instead below using the class name created here:
            string fullClassNameForImplementation = crsTransformationAdapterDotSpatial.GetType().FullName;
            

            // Now creating the composite with weighted average, using the above two
            // leaf adapters, using two different create methods:
            crsTransformationAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(new List<CrsTransformationAdapterWeight> {
                CrsTransformationAdapterWeightFactory.Create().CreateFromInstance(crsTransformationAdapterProjNet, 1.0),
                CrsTransformationAdapterWeightFactory.Create().CreateFromStringWithFullClassNameForImplementation(fullClassNameForImplementation, 2.0)
            });

            // public properties
            crsTransformationAdapteeType = crsTransformationAdapter.AdapteeType;
            isComposite = crsTransformationAdapter.IsComposite;
            longNameOfImplementation = crsTransformationAdapter.LongNameOfImplementation;
            shortNameOfImplementation = crsTransformationAdapter.ShortNameOfImplementation;

            // public methods:
            transformationAdapterChildren = crsTransformationAdapter.TransformationAdapterChildren;

            crsTransformationResult = crsTransformationAdapter.Transform(crsCoordinate, crsIdentifier);
            crsTransformationResult = crsTransformationAdapter.Transform(crsCoordinate, epsgNumber);
            crsTransformationResult = crsTransformationAdapter.Transform(crsCoordinate, crsCode);

            crsCoordinate = crsTransformationAdapter.TransformToCoordinate(crsCoordinate, crsIdentifier);
            crsCoordinate = crsTransformationAdapter.TransformToCoordinate(crsCoordinate, epsgNumber);
            crsCoordinate = crsTransformationAdapter.TransformToCoordinate(crsCoordinate, crsCode);
        }
        
        private void crsTransformationResultCode() {
            crsCoordinate = crsTransformationResult.InputCoordinate;
            crsCoordinate = crsTransformationResult.OutputCoordinate;
            isSuccess = crsTransformationResult.IsSuccess;
            isReliable = crsTransformationResult.IsReliable(2, 0.01);
            crsTransformationResults = crsTransformationResult.TransformationResultChildren;
            exception = crsTransformationResult.Exception;
            crsTransformationAdapter = crsTransformationResult.CrsTransformationAdapterResultSource;
            crsTransformationResultStatistic = crsTransformationResult.CrsTransformationResultStatistic;
        }

        private void crsTransformationResultStatisticCode() {
            crsCoordinate = crsTransformationResultStatistic.CoordinateAverage;
            crsCoordinate = crsTransformationResultStatistic.CoordinateMedian;
            isStatisticsAvailable = crsTransformationResultStatistic.IsStatisticsAvailable;
            crsTransformationResults = crsTransformationResultStatistic.AllCrsTransformationResults;
            numberOfPotentiallySuccesfulResults = crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults;
            maxDifferenceForXEastingLongitude = crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude;
            maxDifferenceForYNorthingLatitude = crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude;
        }

        private void crsTransformationAdapterSubtypesCode() {
            // The below used code is implemented with F#
            // which normally uses explicit interfaces.
            // The F# code requires some additional coding 
            // to make the implicit interfaces work as below
            // i.e. the methods can be invoked not only with 
            // an interface typed object but also with 
            // an object typed with some subtype (class)

            var c = new CrsTransformationAdapterDotSpatial();
            CrsTransformationAdapterBase b = c;
            CrsTransformationAdapterBaseLeaf l = c;
            c.Transform(null, 123);
            c.TransformToCoordinate(null, 123);
            b.Transform(null, 123);
            b.TransformToCoordinate(null, 123);
            l.Transform(null, 123);
            l.TransformToCoordinate(null, 123);

            // Previously (before the git commit when this comment was added)
            // the above methods could not be compiled i.e. 
            // the Transform methods were only available 
            // when assigning the subtypes to the interface
            // i.e. the code below worked before but not 
            // the transform method calls above before these lines with comments
            ICrsTransformationAdapter i = c;
            i.Transform(null, 123);
            i.TransformToCoordinate(null, 123);
        }

        private CrsIdentifier crsIdentifier;
        private CrsCoordinate crsCoordinate;
        private double xEastingLongitude, yNorthingLatitude, maxDifferenceForXEastingLongitude, maxDifferenceForYNorthingLatitude;
        private int epsgNumber;
        private string crsCode;
        private bool isEpsgCode;
        private ICrsTransformationAdapter crsTransformationAdapter;
        private CrsTransformationAdapteeType crsTransformationAdapteeType;
        private bool isComposite, isSuccess, isReliable, isStatisticsAvailable;
        private string longNameOfImplementation;
        private string shortNameOfImplementation;
        private IList<ICrsTransformationAdapter> transformationAdapterChildren;
        private CrsTransformationResult crsTransformationResult;
        private Exception exception;
        private CrsTransformationResultStatistic crsTransformationResultStatistic;
        private IList<CrsTransformationResult> crsTransformationResults;
        private int numberOfPotentiallySuccesfulResults;
        private CrsTransformationAdapterCompositeFactory crsTransformationAdapterCompositeFactory;
    }
}