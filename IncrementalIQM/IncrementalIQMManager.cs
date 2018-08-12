using System;
using System.Collections.Generic;
using System.IO;

namespace IncrementalIQM
{
    public delegate void HandleResultsMethod(int totalRecords, double iqm);

    public class IncrementalIQMManager
    {
        public string FilePath { get; set; }
        public CalculationType CalcType { get; set; }
        public InsertionType InsertType { get; set; }

        private HandleResultsMethod HandleResults;

        private delegate void InsertMethod(List<int> data, int newItem);
        private delegate double CalculateMethod(List<int> data);

        #region Constructors
        public IncrementalIQMManager(string path, 
            CalculationType calcType = CalculationType.Modified,
            InsertionType insertionType = InsertionType.InsertInPlace,
            HandleResultsMethod handleResults = null)
        {
            FilePath = path;
            CalcType = calcType;
            InsertType = insertionType;
            HandleResults = handleResults ?? HandleResultsDefault;
        }
        public IncrementalIQMManager()
            : this(null, CalculationType.Modified, InsertionType.InsertInPlace, null) { }
        public IncrementalIQMManager(string path)
            : this(path, CalculationType.Modified, InsertionType.InsertInPlace, null) { }
        public IncrementalIQMManager(string path, HandleResultsMethod handleResults)
                    : this(path, CalculationType.Modified, InsertionType.InsertInPlace, handleResults) { }

        #endregion

        #region Insertion Methods
        /// <summary>
        /// Inserts a new item into the correct sorted position in an existing list
        /// </summary>
        /// <param name="data">The existing list</param>
        /// <param name="newItem">The new item to be inserted</param>
        private static void InsertInPlace(List<int> data, int newItem)
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
        /// Adds a new item to the list, then sorts the list. This is how the original code did it, and it's really slow & inefficient.
        /// It's only being included here for performance testing (so we can compare the original run to the improved run)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="newItem"></param>
        private static void AddAndSort(List<int> data, int newItem)
        {
            data.Add(newItem);
            data.Sort();
        }

        #endregion

        #region Delegate Setters

        /// <summary>
        /// Sets the insertion method for execution. Normally with only two options I'd just use a conditional, 
        /// but this allows for additional insertion options later
        /// </summary>
        /// <returns></returns>
        private static InsertMethod SetInsertionMethod(InsertionType insertType)
        {
            switch (insertType)
            {
                case InsertionType.AddAndSort:
                    return AddAndSort;
                default:
                    return InsertInPlace;
            }
        }

        private static CalculateMethod SetCalculateMethod(CalculationType calcType)
        {
            switch(calcType)
            {
                case CalculationType.Original:
                    return IQM.CalculateOriginalIQM;
                case CalculationType.Standard:
                    return IQM.CalculateStandardIQM;
                default:
                    return IQM.CalculateModifiedIQM;
            }
        }

        #endregion

        #region Execute Methods
        public double Execute(string filePath, CalculationType calcType, InsertionType insertType)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(paramName: "FilePath", message: "File Path not defined");

            var InsertData = SetInsertionMethod(insertType);
            var CalculateMean = SetCalculateMethod(calcType);

            DateTime beforeTime = DateTime.Now;
            
            try
            {
                List<int> data = new List<int>();
                using (StreamReader sr = new StreamReader(filePath))
                {
                    var line = "";
                    // reads through each line in the file & calculates the intermediate IQM
                    while ((line = sr.ReadLine()) != null)
                    {
                        InsertData(data, Convert.ToInt32(line));
                        if (data.Count >= 4)
                        {
                            var mean = CalculateMean(data);
                            HandleResults(data.Count, mean);
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

            return diff.TotalMilliseconds;
        }

        public double Execute() { return Execute(FilePath, CalcType, InsertType); }
        public double Execute(string path) { return Execute(path, CalcType, InsertType); }
        public double Execute(CalculationType calcType) { return Execute(FilePath, calcType, InsertType); }
        public double Execute(InsertionType insertType) { return Execute(FilePath, CalcType, insertType); }
        public double Execute(string path, CalculationType calcType) { return Execute(path, calcType, InsertType); }

        #endregion

        public void HandleResultsDefault(int totalRecords, double iqm)
        {
            Console.WriteLine($"Index => {totalRecords}, Mean => {iqm:F2}");
        }
    }
}
