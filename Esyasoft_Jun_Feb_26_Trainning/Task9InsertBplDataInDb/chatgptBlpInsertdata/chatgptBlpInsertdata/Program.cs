using System;
class Program
{
    static void Main()
    {
        BatchProcessor processor = new BatchProcessor();
        processor.Start();
        Console.WriteLine("Batch processing completed.");
    }
}
