using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IncrementalIQM
{
    public static class IQM
    {
        /// <summary>
        /// Calculates the Interquartile Mean (ie. the mean of the middle 50%) of a dataset. 
        /// </summary>
        /// <param name="data">The data to be used in the calculation - MUST BE PRE-SORTED</param>
        /// <returns></returns>
        public static double CalculateStandardIQM(List<int> data)
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
        /// Calculates a modified Interquartile Mean, which accounts for the fractions at the edge of the quartile, 
        /// rather than truncating like standard IQM does
        /// </summary>
        /// <param name="data">The data to be used in the calculation - MUST BE PRE-SORTED</param>
        /// <returns></returns>
        public static double CalculateModifiedIQM(List<int> data)
        {
            var oneQuarter = data.Count / 4.0;
            var startIndex = (int) Math.Ceiling(oneQuarter); 
            var recordsToCount = data.Count - (startIndex * 2); 

            var innerHalf = data.GetRange(startIndex, recordsToCount); 
            // the remaining fraction to be added from the edge values, yielding zero if oneQuarter is a whole number
            var edgeFraction = (1 - (oneQuarter - Math.Floor(oneQuarter))) % 1; 
            var edgeValues = (data[startIndex - 1] + data[startIndex + recordsToCount]) * edgeFraction; 
            // get the sum, with the edges
            var modifiedSum = innerHalf.Sum() + edgeValues; 
            // get the modified IQM, which SHOULD be as close as possible to the true inner half
            var modifiedMean = modifiedSum / (oneQuarter * 2); 
            return modifiedMean;
        }

        /// <summary>
        /// Calcuates modified Interquartile Mean, using the original, unmodified algorithm. Useful for side-by-side testing
        /// </summary>
        /// <param name="data">The data to be used in the calculation - MUST BE PRE-SORTED</param>
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
