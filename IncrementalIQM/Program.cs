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
            var IQMMgr = new IncrementalIQMManager("data.txt");
            var totalMs = IQMMgr.Execute();
            Console.WriteLine($"Total Time (milliseconds): {totalMs}");
        }
    }
}