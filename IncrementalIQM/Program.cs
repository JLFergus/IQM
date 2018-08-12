using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace IncrementalIQM
{

    class Program
    {
        static readonly string _inputPath = "data.txt";

        static void Main()
        {
            var IQMMgr = new IncrementalIQMManager(_inputPath);
            var totalMs = IQMMgr.Execute();

            Console.WriteLine($"Total Time (milliseconds): {totalMs}");
        }
    }
}