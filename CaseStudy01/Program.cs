// Case Study 01 - Multithreading 
// Updated: 2025-06-25
using System;
using System.IO;
using CalculatingFunctions;
using System.Threading;
using System.Diagnostics;

class Program
{
    static decimal[] data = new decimal[11000001];
    static decimal result = 0;
    static readonly object resultLock = new object();

    private static void ThreadWork(int startIndex, int endIndex, CalClass CF)
    {
        decimal localResult = 0;

        for (int i = 0; i < 30; i++)
        {
            int index = startIndex;
            while (index < endIndex)
            {
                localResult += CF.Calculate1(ref data, ref index);
            }
        }

        lock (resultLock)
        {
            result += localResult;
        }
    }

    private static void LoadData()
    {
        Console.WriteLine("Loading data...");
        FileStream fs = new FileStream("data.bin", FileMode.Open);
        BinaryReader br = new BinaryReader(fs);
        for (int i = 0; i < data.Length; i++)
        {
            Single f = br.ReadSingle();
            data[i] = (decimal)(f * 36);
        }
        Console.WriteLine("Data loaded successfully.\n\n");
    }

    private static void Main(string[] args)
    {
        LoadData();
        Console.WriteLine("Calculation start ...");

        int Lenght = 10000000;

        Thread Th1 = new Thread(() => ThreadWork(0, Lenght / 3, new CalClass()));
        Thread Th2 = new Thread(() => ThreadWork(Lenght / 3, 2 * (Lenght / 3), new CalClass()));
        Thread Th3 = new Thread(() => ThreadWork(2 * (Lenght / 3), Lenght, new CalClass()));

        Stopwatch _st = new Stopwatch();
        _st.Start();

        Th1.Start();
        Th2.Start();
        Th3.Start();

        Th1.Join(); 
        Th2.Join(); 
        Th3.Join(); 

        _st.Stop();
        Console.WriteLine($"Calculation finished in {_st.ElapsedMilliseconds} ms. Result: {result.ToString("F25")}");
    }
}