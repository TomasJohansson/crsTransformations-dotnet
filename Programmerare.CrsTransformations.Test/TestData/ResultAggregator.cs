using System.Collections.Generic;
using System.IO;

namespace Programmerare.CrsTransformations.TestData
{
// TODO: figure out a better class name
class ResultAggregator {

    private IList<FileWithRows> listOfFileWithRows = new List<FileWithRows>();

    public void addRowsFromFile(
        IList<string> rowsFromFile,
        FileInfo sourceFile
    ) {
        FileWithRows fileWithRows = new FileWithRows(sourceFile, rowsFromFile);
        listOfFileWithRows.Add(fileWithRows);
        ISet<int> indexes = fileWithRows.getIndexesForRowsWithSignificantDifference(fileWithRows, 0.001);
    }

    public ISet<int> getIndexesForRowsWithSignificantDifference(
        double deltaValueForDifferencesToIgnore
    ) {
        ISet<int> indexes = new HashSet<int>();
        for (int i = 0; i < listOfFileWithRows.Count-1; i++) {
            for (int j = i+1; j < listOfFileWithRows.Count; j++) {
                FileWithRows fileWithRows_i = listOfFileWithRows[i];
                FileWithRows fileWithRows_j = listOfFileWithRows[j];
                ISet<int> indexesForRowsWithSignificantDifference = fileWithRows_i.getIndexesForRowsWithSignificantDifference(fileWithRows_j, deltaValueForDifferencesToIgnore);
                indexes.UnionWith(indexesForRowsWithSignificantDifference);
            }
        }
        return indexes;
    }

    class FileWithRows {
        private FileInfo sourceFile;
        private IList<string> rowsFromFile;

        public FileWithRows(
            FileInfo sourceFile,
            IList<string> rowsFromFile
        ) {
            this.sourceFile = sourceFile;
            this.rowsFromFile = rowsFromFile;
        }

        public ISet<int> getIndexesForRowsWithSignificantDifference(
            FileWithRows that,
            double deltaValueForDifferencesToIgnore
        ) {
            ISet<int> indexes = new HashSet<int>();
            for (int fileRowIndex = 0; fileRowIndex < rowsFromFile.Count; fileRowIndex++) {
                string thisRow = this.rowsFromFile[fileRowIndex];
                string thatRow = that.rowsFromFile[fileRowIndex];
                TestResultItem t1 = new TestResultItem(thisRow);
                TestResultItem t2 = new TestResultItem(thatRow);
                DifferenceWhenComparingCoordinateValues diff = t1.isDeltaDifferenceSignificant(t2, deltaValueForDifferencesToIgnore);
                if(diff == DifferenceWhenComparingCoordinateValues.SIGNIFICANT_VALUE_DIFFERENCE) {
                    indexes.Add(fileRowIndex);
                }
            }
            return indexes;
        }
    }
}
}