using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IncrementalIQM.Test
{
    // This is a blatant misuse of the Unit Test framework, but I hope you'll forgive me ;)
    [TestClass]
    [Ignore]
    [TestCategory("Performance")]
    public class PerformanceTests
    {
        readonly string PerformanceTestInputFilePath = "..\\..\\performance-data.txt";

        // This tests the calculation methods against each others
        // It runs each method ten times, outputting the results to a file, then averages the results of all ten runs
        [TestMethod]
        public void TestCalculationMethods()
        {
            using (var sw = new StreamWriter("CalculationMethodPerformance.csv"))
            {
                try
                {
                    sw.WriteLine("Calculation Method, Insertion Method,Run 1, Run 2, Run 3, Run 4, Run 5, Run 6, Run 7, Run 8, Run 9, Run 10, Average");
                    TestRun(CalculationType.Original, InsertionType.InsertInPlace, 10, sw);
                    TestRun(CalculationType.Modified, InsertionType.InsertInPlace, 10, sw);
                    TestRun(CalculationType.Standard, InsertionType.InsertInPlace, 10, sw);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Could not write to the output file:");
                    Console.WriteLine(e.Message);
                }
            }
        }

        // This compares the two insertion methods, both using the standard caclulation method, since it's fastest
        // Again, we'll run both insertion methods ten times each, tehn report the average
        [TestMethod]
        public void TestInsertionMethods()
        {
            using (var sw = new StreamWriter("InsertionMethodPerformance.csv"))
            {
                try
                {
                    sw.WriteLine("Calculation Method, Insertion Method,Run 1, Run 2, Run 3, Run 4, Run 5, Run 6, Run 7, Run 8, Run 9, Run 10, Average");
                    TestRun(CalculationType.Standard, InsertionType.AddAndSort, 10, sw);
                    TestRun(CalculationType.Standard, InsertionType.InsertInPlace, 10, sw);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not write to the output file:");
                    Console.WriteLine(e.Message);
                }
            }
        }

        // This one will test all permutations of insertion method and calculation method,
        // Ten runs each, plus the average
        [TestMethod]
        public void TestAll()
        {
            using (var sw = new StreamWriter("CompletePerformanceTest.csv"))
            {
                try
                {
                    sw.WriteLine("Calculation Method, Insertion Method,Run 1, Run 2, Run 3, Run 4, Run 5, Run 6, Run 7, Run 8, Run 9, Run 10, Average");

                    TestRun(CalculationType.Original, InsertionType.AddAndSort, 10, sw);
                    TestRun(CalculationType.Modified, InsertionType.AddAndSort, 10, sw);
                    TestRun(CalculationType.Standard, InsertionType.AddAndSort, 10, sw);

                    TestRun(CalculationType.Original, InsertionType.InsertInPlace, 10, sw);
                    TestRun(CalculationType.Modified, InsertionType.InsertInPlace, 10, sw);
                    TestRun(CalculationType.Standard, InsertionType.InsertInPlace, 10, sw);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not write to the output file:");
                    Console.WriteLine(e.Message);
                }
            }
        }


        private void TestRun(CalculationType calcType, InsertionType insertType, int runCount, StreamWriter sw)
        {
            sw.Write($"{calcType.ToString()},{insertType.ToString()},");
            var iqmm = new IncrementalIQMManager(PerformanceTestInputFilePath, calcType, insertType);
            var runtimes = new List<double>();
            for (var i = 0; i<runCount; i++)
            {
                var runtime = iqmm.Execute();
                runtimes.Add(runtime);
                sw.Write($"{runtime},");
            }
            var avgRuntime = runtimes.Sum() / runtimes.Count;
            sw.WriteLine(avgRuntime);
        }
    }
}
