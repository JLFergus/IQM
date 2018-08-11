using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace IncrementalIQM
{
    class Program
    {
        /// <summary>
        /// Inserts a new item into the correct sorted position in an existing list
        /// </summary>
        /// <param name="data">The existing list</param>
        /// <param name="newItem">The new item to be inserted</param>
        static void InsertData(List<int> data, int newItem)
        {
            /* Rather than just adding the new item to the end, then re-sorting the whole list, I'm doing a single insertion.
               List.Sort() uses an insertion sort for small datasets anyway, so I'm effectively skipping to the very last step in the sort.
               This is the largest optimization, reducing runtime by about 60% */
            var inserted = false;
            for (var i = 0; i < data.Count; i++)
            {
                if (data[i] >= newItem)
                {
                    data.Insert(i, newItem);
                    inserted = true;
                    break;
                }
            }
            /* if we make it all the way to the end of the list without inserting, 
            that means this is the largest number, so we just add it to the end */
            if (!inserted)
                data.Add(newItem);
        }

        /// <summary>
        /// Calculates the Interquartile Mean (ie. the mean of the middle 50%) of a dataset. 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static double CalculateTrueIQM(List<int> data)
        {
            // extract the middle half
            var oneQuarter = (int)Math.Floor(data.Count / 4.0);
            var innerHalfCount = data.Count - (2 * oneQuarter);

            // because lists are zero-indexed, our oneQuarter value is also the index of the first value in our quartile set
            var quartileData = data.GetRange(oneQuarter, innerHalfCount);

            var sum = quartileData.Sum();
            var mean = sum / (double)quartileData.Count;

            return mean;
        }

        /// <summary>
        /// Calculates a modified Interquartile Mean. It does some tricky calculations that I don't fully understand to come up with 
        /// a value similar to the IQM, but usually off by a fraction.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static double CalculateModifiedIQM(List<int> data)
        {
            double oneQuarter = data.Count() / 4.0;
            int startIndex = (int)Math.Ceiling(oneQuarter) - 1;
            int recordsToCount = (int)Math.Floor(oneQuarter * 3) - startIndex + 1;


            var innerHalf = data.GetRange(startIndex, recordsToCount);
            var factor = oneQuarter - ((innerHalf.Count() / 2.0) - 1);

            // chop off the ends and sum up the rest. 
            var subsetSum = innerHalf.GetRange(1, innerHalf.Count - 2).Sum();

            double modifiedMean = (subsetSum + (innerHalf.First() + innerHalf.Last()) * factor) / (2 * oneQuarter);

            return modifiedMean;
        }

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
                        InsertData(data, Convert.ToInt32(line));
                        if (data.Count >= 4)
                        {
                            //var mean = CalculateTrueIQM(data);
                            //Console.WriteLine($"Index => {data.Count()}, Mean => {mean:F2}");

                            var modifiedMean = CalculateModifiedIQM(data);
                            Console.WriteLine($"Index => {data.Count()}, Mean => {modifiedMean:F2}");

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