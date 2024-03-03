using System;
using System.Text;
using System.Text.Json;
using SimdJsonSharp;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using System.Diagnostics;
using Newtonsoft.Json;
//using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;

namespace BenchmarkingJsonParser
{
    public class Program
    {
        static unsafe void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a JSON file as an argument.");

                return;
            }

            string jsonFilePath = args[0];

            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"File {jsonFilePath} does not exist.");
                return;
            }

            string jsonContent = File.ReadAllText(jsonFilePath);
            ReadOnlySpan<byte> bytes = Encoding.UTF8.GetBytes(jsonContent);

        
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ulong numbersCount = 0;
            fixed (byte* ptr = bytes)
            {
                using (ParsedJsonN doc = SimdJsonN.ParseJson(ptr, bytes.Length))
                {
                    Console.WriteLine($"Is json valid: {doc.IsValid}\n");

                    using (var iterator = new ParsedJsonIteratorN(doc))
                    {
                        while (iterator.MoveForward())
                        {
                            if (iterator.IsString || iterator.IsInteger)
                                numbersCount++;
                        }
                    }
                }
            }

            stopwatch.Stop();
            Console.WriteLine(numbersCount);
            Console.WriteLine($"SimdJsonSharp.Bindings - Time taken: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}   
