package com.programmerare.com.programmerare.testData;

import java.io.File;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

// TODO: figure out a better class name
final class ResultAggregator {

    private final List<FileWithRows> listOfFileWithRows = new ArrayList<FileWithRows>();

    public void addRowsFromFile(
        final List<String> rowsFromFile,
        final File sourceFile
    ) {
        final FileWithRows fileWithRows = new FileWithRows(sourceFile, rowsFromFile);
        listOfFileWithRows.add(fileWithRows);
        final Set<Integer> indexes = fileWithRows.getIndexesForRowsWithSignificantDifference(fileWithRows, 0.001);
    }

    public Set<Integer> getIndexesForRowsWithSignificantDifference(
        final double deltaValueForDifferencesToIgnore
    ) {
        final Set<Integer> indexes = new HashSet<Integer>();
        for (int i = 0; i < listOfFileWithRows.size()-1; i++) {
            for (int j = i+1; j < listOfFileWithRows.size(); j++) {
                final FileWithRows fileWithRows_i = listOfFileWithRows.get(i);
                final FileWithRows fileWithRows_j = listOfFileWithRows.get(j);
                final Set<Integer> indexesForRowsWithSignificantDifference = fileWithRows_i.getIndexesForRowsWithSignificantDifference(fileWithRows_j, deltaValueForDifferencesToIgnore);
                indexes.addAll(indexesForRowsWithSignificantDifference);
            }
        }
        return indexes;
    }

    final class FileWithRows {
        private final File sourceFile;
        private final List<String> rowsFromFile;

        public FileWithRows(
            final File sourceFile,
            final List<String> rowsFromFile
        ) {
            this.sourceFile = sourceFile;
            this.rowsFromFile = rowsFromFile;
        }

        public Set<Integer> getIndexesForRowsWithSignificantDifference(
            final FileWithRows that,
            final double deltaValueForDifferencesToIgnore
        ) {
            final Set<Integer> indexes = new HashSet<Integer>();
            for (int fileRowIndex = 0; fileRowIndex < rowsFromFile.size(); fileRowIndex++) {
                final String thisRow = this.rowsFromFile.get(fileRowIndex);
                final String thatRow = that.rowsFromFile.get(fileRowIndex);
                final TestResultItem t1 = new TestResultItem(thisRow);
                final TestResultItem t2 = new TestResultItem(thatRow);
                final DifferenceWhenComparingCoordinateValues diff = t1.isDeltaDifferenceSignificant(t2, deltaValueForDifferencesToIgnore);
                if(diff == DifferenceWhenComparingCoordinateValues.SIGNIFICANT_VALUE_DIFFERENCE) {
                    indexes.add(fileRowIndex);
                }
            }
            return indexes;
        }
    }
}