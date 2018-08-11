using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace IncrementalIQM
{

    class Program
    {
        

        static void Main()
        {
            DateTime beforeTime = DateTime.Now;

            try
            {
                List<int> data = new List<int>();
                using (StreamReader sr = new StreamReader("data.txt"))
                {
                    var line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        IQM.InsertData(data, Convert.ToInt32(line));
                        if (data.Count >= 4)
                        {
                            //var mean = IQM.CalculateTrueIQM(data);
                            //Console.WriteLine($"Index => {data.Count()}, Mean => {mean:F2}");

                            var modifiedMean = IQM.CalculateModifiedIQM(data);
                            Console.WriteLine($"Index => {data.Count()}, Mean => {modifiedMean:F2}");

                            //var originalMean = IQM.CalculateOriginalIQM(data);
                            //Console.WriteLine($"Index => {data.Count()}, Mean => {originalMean:F2}");

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            DateTime afterTime = DateTime.Now;
            TimeSpan diff = afterTime - beforeTime;
            Console.WriteLine("Total Milliseconds: {0}", diff.TotalMilliseconds);
        }
    }
}