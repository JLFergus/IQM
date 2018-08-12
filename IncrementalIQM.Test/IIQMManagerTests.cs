using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace IncrementalIQM.Test
{
    [TestClass]
    public class IIQMManagerTests
    {
        // I could probably configure the tests to copy the data file into the debug folder,
        // but I don't have time for that, and these are tests
        const string TestDataPath = "..\\..\\test-data.txt";


        [TestMethod]
        public void Constructor_ShouldHaveAllProperties()
        {            
            var defaultIIQM = new IncrementalIQMManager();
            var pathIIQM = new IncrementalIQMManager(TestDataPath);
            var customIIQM = new IncrementalIQMManager(TestDataPath, CalculationType.Standard, InsertionType.AddAndSort);

            Assert.IsNull(defaultIIQM.FilePath);
            Assert.AreEqual(defaultIIQM.CalcType, CalculationType.Modified);
            Assert.AreEqual(defaultIIQM.InsertType, InsertionType.InsertInPlace);

            Assert.AreEqual(pathIIQM.FilePath, TestDataPath);
            Assert.AreEqual(pathIIQM.CalcType, CalculationType.Modified);
            Assert.AreEqual(pathIIQM.InsertType, InsertionType.InsertInPlace);

            Assert.AreEqual(customIIQM.FilePath, TestDataPath);
            Assert.AreEqual(customIIQM.CalcType, CalculationType.Standard);
            Assert.AreEqual(customIIQM.InsertType, InsertionType.AddAndSort);
        }

        [TestMethod]
        public void Execute_Standard_ShouldCalculateIQMCorrectly()
        {
            var resultsDict = new Dictionary<int, double>();
            void SaveToDictionary(int totalRecords, double iqm)
            {
                resultsDict.Add(totalRecords, iqm);
            }

            var standardIQM = new IncrementalIQMManager(TestDataPath, SaveToDictionary);
            standardIQM.Execute(CalculationType.Standard);

            Assert.AreEqual(resultsDict.Count, TestResults.StandardResults.Count);
            foreach (var key in resultsDict.Keys)
                Assert.AreEqual(resultsDict[key], TestResults.StandardResults[key]);
        }

        [TestMethod]
        public void Execute_Modified_ShouldEqualOriginal()
        {
            var originalResults = new Dictionary<int, double>();
            void SaveToOriginalResultsDictionary(int totalRecords, double iqm)
            {
                originalResults.Add(totalRecords, iqm);
            }
            var modifiedResults = new Dictionary<int, double>();
            void SaveToModifiedResultsDictionary(int totalRecords, double iqm)
            {
                modifiedResults.Add(totalRecords, iqm);
            }

            var IQM = new IncrementalIQMManager(TestDataPath);
            IQM.Execute(SaveToOriginalResultsDictionary);
            IQM.Execute(SaveToModifiedResultsDictionary);

            Assert.AreEqual(originalResults.Count, modifiedResults.Count);
            foreach (var key in originalResults.Keys)
                Assert.AreEqual(originalResults[key], modifiedResults[key]);
        }

        [TestMethod]
        public void Execute_InsertionMethod_ShouldNotAffectResults()
        {
            var insertInPlaceResults = new Dictionary<int, double>();
            void SaveInsertInPlaceResults(int totalRecords, double iqm)
            {
                insertInPlaceResults.Add(totalRecords, iqm);
            }
            var addAndSortResults = new Dictionary<int, double>();
            void SaveToAddAndSortResults(int totalRecords, double iqm)
            {
                addAndSortResults.Add(totalRecords, iqm);
            }
            var IQM = new IncrementalIQMManager();

            // should work with every calculation type

            // Original
            IQM.Execute(TestDataPath, CalculationType.Original, InsertionType.AddAndSort, SaveToAddAndSortResults);
            IQM.Execute(TestDataPath, CalculationType.Original, InsertionType.InsertInPlace, SaveInsertInPlaceResults);

            Assert.AreEqual(insertInPlaceResults.Count, addAndSortResults.Count);
            foreach (var key in insertInPlaceResults.Keys)
                Assert.AreEqual(insertInPlaceResults[key], addAndSortResults[key]);

            // Modified
            IQM.Execute(TestDataPath, CalculationType.Modified, InsertionType.AddAndSort, SaveToAddAndSortResults);
            IQM.Execute(TestDataPath, CalculationType.Modified, InsertionType.InsertInPlace, SaveInsertInPlaceResults);

            Assert.AreEqual(insertInPlaceResults.Count, addAndSortResults.Count);
            foreach (var key in insertInPlaceResults.Keys)
                Assert.AreEqual(insertInPlaceResults[key], addAndSortResults[key]);

            // Standard
            IQM.Execute(TestDataPath, CalculationType.Standard, InsertionType.AddAndSort, SaveToAddAndSortResults);
            IQM.Execute(TestDataPath, CalculationType.Standard, InsertionType.InsertInPlace, SaveInsertInPlaceResults);

            Assert.AreEqual(insertInPlaceResults.Count, addAndSortResults.Count);
            foreach (var key in insertInPlaceResults.Keys)
                Assert.AreEqual(insertInPlaceResults[key], addAndSortResults[key]);
        }
    }
}
