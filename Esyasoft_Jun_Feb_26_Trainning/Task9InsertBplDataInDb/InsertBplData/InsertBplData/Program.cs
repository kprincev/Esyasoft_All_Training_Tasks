using InsertBplData;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;


class Program
{
    public static void Main(string[] args)
    {
        Stopwatch sw = new Stopwatch();
        var batchProcessor = new BatchProcessor();
        batchProcessor.Start();
        Console.WriteLine("Program completed in: " + sw.Elapsed);
        Console.WriteLine("Milliseconds: " + sw.ElapsedMilliseconds);
    }
}