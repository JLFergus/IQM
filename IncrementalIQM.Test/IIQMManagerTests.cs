using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IncrementalIQM;

namespace IncrementalIQM.Test
{
    [TestClass]
    public class IIQMManagerTests
    {
        const string TestDataPath = "testData.txt";


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
    }
}
