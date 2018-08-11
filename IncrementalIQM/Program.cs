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
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        data.Add(Convert.ToInt32(line));
                        data.Sort();
                        
                        if (data.Count() >= 4)
                        {
                            double q = data.Count() / 4.0;
                            int i = (int)Math.Ceiling(q) - 1;
                            int c = (int)Math.Floor(q*3) - i + 1;

                            List<int> ys = data.GetRange(i, c);
                            double factor = q - ((ys.Count() / 2.0) - 1);
                            
                            int sum = 0;
                            
                            var listEnumerator = ys.GetEnumerator();
                            for (var j = 0; listEnumerator.MoveNext() == true; j++)
                            {
                                if (j == 0)
                                {
                                    continue;
                                }
                                else if (j == (ys.Count() - 1))
                                {
                                    break; 
                                }
                               
                                sum += listEnumerator.Current;
                            }

                            double mean = (sum + (ys.First() + ys.Last()) * factor) / (2 * q);
                            Console.WriteLine("Index => {0}, Mean => {1:F2}", data.Count(), mean);
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