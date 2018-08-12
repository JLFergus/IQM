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
            var IQMMgr = new IncrementalIQMManager("data.txt", 
                (totalRecords, iqm) => Console.WriteLine($"{totalRecords} total Records, IQM: {iqm:F2}"));
            var totalMs = IQMMgr.Execute();
            Console.WriteLine($"Total Time (milliseconds): {totalMs}");
        }
    }
}