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
            // Rather than just adding the new item to the end, then re-sorting the whole list, I'm doing a single insertion
            // List.Sort() uses an insertion sort for small datasets anyway, so I'm effectively skipping to the very last step in the sort.
            var inserted = false;
            for (var i = 0; i<data.Count; i++)
            {
                if(data[i] >= newItem)
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
        static double CalculateIQM(List<int> data, StreamWriter sw)
        {
            // extract the middle half
            var oneQuarter = (int)Math.Floor(data.Count / 4.0);
            var InterQuartile = data.Count - (2 * oneQuarter);
            var quartileData = data.GetRange(oneQuarter, InterQuartile);

            // calculate the mean
            //var sum = 0;
            //quartileData.ForEach(item => sum += item);

            var sum = quartileData.Sum();
            var mean = sum / (double) quartileData.Count;

            // todo: I'm putting the output in here for now, but only because I want to see the quartile count and that's no longer accessible
            sw.WriteLine($"{data.Count}, {mean}, {quartileData.Count}");
            Console.WriteLine("Index => {0}, Mean => {1:F2}", data.Count(), mean);

            return mean;
        }

        static void Main()
        {
            DateTime beforeTime = DateTime.Now;
            
            try
            {
                List<int> data = new List<int>();
                using (StreamReader sr = new StreamReader("data.txt"))
                {
                    using (StreamWriter sw = new StreamWriter("data.csv"))
                    {
                        sw.WriteLine("Total Items, Mean, Calculated Items");

                        String line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            InsertData(data, Convert.ToInt32(line));
                            if (data.Count >= 4)
                            {
                                var mean = CalculateIQM(data, sw);
                                // todo: the output will go here once it's cleaned up
                            }

                            //if (data.Count() >= 4)
                            //{
                            //    double q = data.Count() / 4.0;
                            //    int i = (int)Math.Ceiling(q) - 1;
                            //    int c = (int)Math.Floor(q * 3) - i + 1;


                            //    List<int> ys = data.GetRange(i, c);
                            //    double factor = q - ((ys.Count() / 2.0) - 1);

                            //    int sum = 0;

                            //    var listEnumerator = ys.GetEnumerator();
                            //    for (var j = 0; listEnumerator.MoveNext() == true; j++)
                            //    {
                            //        if (j == 0)
                            //        {
                            //            continue;
                            //        }
                            //        else if (j == (ys.Count() - 1))
                            //        {
                            //            break;
                            //        }

                            //        sum += listEnumerator.Current;
                            //    }

                            //    double mean = (sum + (ys.First() + ys.Last()) * factor) / (2 * q);
                            //    sw.WriteLine($"{data.Count}, {mean}, {ys.Count}");
                            //    Console.WriteLine("Index => {0}, Mean => {1:F2}", data.Count(), mean);

                            // }
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
            Console.ReadLine();
        }
    }
}