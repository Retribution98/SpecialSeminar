using OneDimensionalCutting.Algorithms;
using OneDimensionalCutting.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Xunit;

namespace OneDimensionalCutting.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string zipPath = @"..\..\..\Task1.zip";
            Debug.WriteLine("Name \t\t\t\t Min \t Default \t Special \t Iterate");
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    using (var reader = new StreamReader(entry.Open()))
                    {
                        var input = reader.ReadLine()
                            .Split(' ')
                            .Where(x => x != null)
                            .Select(x => int.Parse(x))
                            .ToArray();
                        if (input == null || input.Length < 2)
                        {
                            throw new InvalidDataException($"Data in {entry.FullName} is invalid");
                        }
                        var inputDto = new InputData(input[0], input.Skip(1));
                        var min = Math.Ceiling((decimal)inputDto.Blanks.Sum(x => x.Length) / inputDto.Rod.Length);
                        var algorithm = new DefaultAlgorithm();
                        var algorithm2 = new SpecialAlgorithm();
                        //var algorithm3 = new IterateAlgorithm(100, 10);
                        var transposition = algorithm.GetTransposition(inputDto);
                        var transposition2 = algorithm2.GetTransposition(inputDto);
                        //var transposition3 = algorithm3.GetTransposition(inputDto);
                        var countRod = inputDto.GetCountRod(transposition);
                        var countRod2 = inputDto.GetCountRod(transposition2);
                        //var countRod3 = inputDto.GetCountRod(transposition3);

                        Debug.WriteLine($"{entry.Name} => \t {min} \t {countRod}\t\t{countRod2}\t\t");
                    }
                }
            }
        }
    }
}
