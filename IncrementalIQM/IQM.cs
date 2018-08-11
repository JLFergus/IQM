using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IncrementalIQM
{
    public static class IQM
    {
        /// <summary>
        /// Inserts a new item into the correct sorted position in an existing list
        /// </summary>
        /// <param name="data">The existing list</param>
        /// <param name="newItem">The new item to be inserted</param>
        public static void InsertData(List<int> data, int newItem)
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
        public static double CalculateTrueIQM(List<int> data)
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
        public static double CalculateModifiedIQM(List<int> data)
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

        /// <summary>
        /// Calcuates modified Interquartile Mean, using the original, unmodified algorithm. Useful for side-by-side testing
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double CalculateOriginalIQM(List<int> data)
        {
            double q = data.Count() / 4.0;
            int i = (int)Math.Ceiling(q) - 1;
            int c = (int)Math.Floor(q * 3) - i + 1;
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
            double originalMean = (sum + (ys.First() + ys.Last()) * factor) / (2 * q);

            return originalMean;
        }
    }
}
