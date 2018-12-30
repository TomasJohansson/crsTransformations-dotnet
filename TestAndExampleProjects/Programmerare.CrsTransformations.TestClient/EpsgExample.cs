using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;
using static Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4.EpsgNumber;
using System.Collections.Generic;
using System;

namespace Programmerare.CrsTransformations.TestClient {
    public class EpsgExample {
        public EpsgExample() {
            // This file is only used for creating screenshots for the github
            // page when using dropdown (intellisense/code completeion) with Visual Studio
            //EpsgNumber.SWE

            var constants = new List<int> {
            // Example constants used in the github page:
            WORLD__WGS_84__4326,
            WORLD__85_S_TO_85_N__WGS_84__PSEUDO_MERCATOR__3857  ,
            NORTH_AMERICA__NAD83__NAD83__4269  ,
            USA__US_NATIONAL_ATLAS_EQUAL_AREA__2163  ,
            CANADA__NAD83__CANADA_ATLAS_LAMBERT__3978  ,
            UK__GREAT_BRITAIN_MAINLAND_ONSHORE__OSGB_1936__BRITISH_NATIONAL_GRID__ODN_HEIGHT__7405  ,
            IRELAND__ONSHORE__TM65__IRISH_GRID__29902  ,
            AUSTRALIA__GDA__GDA94__4283  ,
            NEW_ZEALAND__ONSHORE__NZGD49__NEW_ZEALAND_MAP_GRID__27200  ,
            SWEDEN__SWEREF99_TM__3006  ,
            FINLAND__19_5_E_TO_20_5_E_ONSHORE_NOMINAL__ETRS89__GK20FIN__3874  ,
            DENMARK__ONSHORE_BORNHOLM__ETRS89__KP2000_BORNHOLM__2198  ,
            NORWAY__ONSHORE__NGO_1948__OSLO__4817  ,
            ICELAND__ONSHORE__REYKJAVIK_1900__4657  ,
            NETHERLANDS__ONSHORE__AMERSFOORT__RD_NEW__28992  ,
            BELGIUM__ONSHORE__BELGE_1972__BELGIAN_LAMBERT_72__31370  ,
            GERMANY__WEST_GERMANY__10_5_E_TO_13_5_E__DHDN__3_DEGREE_GAUSS_KRUGER_ZONE_4__31468  ,
            AUSTRIA__ETRS89__AUSTRIA_LAMBERT__3416 ,
            EUROPE__LIECHTENSTEIN_AND_SWITZERLAND__CH1903__LV03__21781  ,
            };
        }
        
        public void method() {
            Console.WriteLine("number of fields: " + typeof(EpsgNumber).GetFields().Length); // 6733
        }
    }
}
