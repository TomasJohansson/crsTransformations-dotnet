using System.Collections.Generic;
using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI;
using Programmerare.CrsTransformations.CompositeTransformations;

namespace Programmerare.CrsTransformations
{
public abstract class CrsTransformationTestBase {

    protected readonly int epsgNumberForWgs84         = EpsgNumber.WORLD__WGS_84__4326;
    protected readonly int epsgNumberForSweref99TM    = EpsgNumber.SWEDEN__SWEREF99_TM__3006;
    protected readonly int epsgNumberForRT90          = EpsgNumber.SWEDEN__2_5_GON_W__RT90_2_5_GON_V__3021;

    protected List<ICrsTransformationAdapter> crsTransformationAdapterLeafImplementations;
    protected List<ICrsTransformationAdapter> crsTransformationAdapterCompositeImplementations;
    protected List<ICrsTransformationAdapter> crsTransformationAdapterImplementations;

    [SetUp]
    public void SetUp() {

        crsTransformationAdapterLeafImplementations = new List<ICrsTransformationAdapter>{
            new CrsTransformationAdapterDotSpatial(),
            new CrsTransformationAdapterProjNet4GeoAPI(),
            new CrsTransformationAdapterMightyLittleGeodesy()
        };

        crsTransformationAdapterCompositeImplementations = new List<ICrsTransformationAdapter>{
            CrsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage(),
            CrsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian(),
            CrsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess(),
            CrsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(new List<CrsTransformationAdapterWeight>{
                CrsTransformationAdapterWeight.CreateFromInstance(new CrsTransformationAdapterDotSpatial(), 51.0),
                CrsTransformationAdapterWeight.CreateFromInstance(new CrsTransformationAdapterProjNet4GeoAPI(), 52.0),
                CrsTransformationAdapterWeight.CreateFromInstance(new CrsTransformationAdapterMightyLittleGeodesy(), 53.0)
            })
        };
        crsTransformationAdapterImplementations = new List<ICrsTransformationAdapter>();
        crsTransformationAdapterImplementations.AddRange(crsTransformationAdapterLeafImplementations);
        crsTransformationAdapterImplementations.AddRange(crsTransformationAdapterCompositeImplementations);
    }
}
}